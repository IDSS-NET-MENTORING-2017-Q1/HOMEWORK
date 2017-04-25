﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace CrawlerLibrary
{
	public class Crawler
	{
		private int _nestingLevel = 1;
		private string _searchPhrase;

		private IEnumerable<string> _blockedExtensions = new string[] {
			".css", ".js",
			".png", ".jpg", ".jpeg", ".bmp", ".gif", ".svg",
			".mp3", ".wav"
		};

		private Regex _urlRegex = new Regex(@"^http(s)?:\/\/[a-z_0-9\/\-\.\?]+$", RegexOptions.IgnoreCase);

		private Parser _parser;
		private Searcher _searcher;
		private Downloader _downloader;

		public int NestingLevel { get => _nestingLevel; set => _nestingLevel = value; }
		public string SearchPhrase { get => _searchPhrase; set => _searchPhrase = value; }

		public Parser Parser { get => _parser; set => _parser = value; }
		public Searcher Searcher { get => _searcher; set => _searcher = value; }
		public Downloader Downloader { get => _downloader; set => _downloader = value; }
		public IEnumerable<string> BlockedExtensions { get => _blockedExtensions; set => _blockedExtensions = value; }
		public Regex UrlRegex { get => _urlRegex; set => _urlRegex = value; }

		protected void HandleCertificates()
		{
			ServicePointManager.ServerCertificateValidationCallback += (object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors error) =>
			{
				return true;
			};
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
												 | SecurityProtocolType.Tls11
												 | SecurityProtocolType.Tls12
												 | SecurityProtocolType.Ssl3;
		}

		public Crawler()
		{
			HandleCertificates();
		}

		public Crawler(Parser parser, Searcher searcher, Downloader downloader) : this()
		{
			_parser = parser;
			_searcher = searcher;
			_downloader = downloader;
		}

		public IEnumerable<string> Crawl(string url)
		{
			return Crawl(0, url);
		}

		public IEnumerable<string> Crawl(int level, string url)
		{
			List<string> result = new List<string>();
			string pageMarkup = _downloader.DownloadPage(url);
			if (_searcher.FindPhrase(pageMarkup, _searchPhrase))
			{
				result.Add(url);
			}

			if (level + 1 <= _nestingLevel)
			{
				IEnumerable<string> nestedUrls = _parser.GetUrls(pageMarkup)
					.Where(parsedUrl => {
						return _urlRegex.IsMatch(parsedUrl) && !_blockedExtensions.Any(extension => parsedUrl.EndsWith(extension));
					});
				Parallel.ForEach(nestedUrls, (nestedUrl) =>
				{
					result.AddRange(Crawl(level + 1, nestedUrl));
				});
			}

			return result;
		}

		public async Task<IEnumerable<string>> CrawlAsync(string url, CancellationToken token)
		{
			return await CrawlAsync(0, url, token);
		}

		public async Task<IEnumerable<string>> CrawlAsync(int level, string url, CancellationToken token)
		{
			List<string> result = new List<string>();

			string pageMarkup;
			try
			{
				pageMarkup = await _downloader.DownloadPageAsync(url, token);
			}
			catch (WebException ex)
			{
				Debug.WriteLine(ex.Message);
				return null;
			}

			token.ThrowIfCancellationRequested();

			if (_searcher.FindPhrase(pageMarkup, _searchPhrase))
			{
				result.Add(url);
			}

			token.ThrowIfCancellationRequested();

			if (level + 1 <= _nestingLevel)
			{
				IEnumerable<string> nestedUrls = _parser.GetUrls(pageMarkup)
					.Where(parsedUrl => {
						return _urlRegex.IsMatch(parsedUrl) && !_blockedExtensions.Any(g => parsedUrl.EndsWith(g));
					}); ;

				token.ThrowIfCancellationRequested();

				IEnumerable<Task> crawlTasks = nestedUrls.Select(nestedUrl =>
				{
					token.ThrowIfCancellationRequested();
					return CrawlAsync(level + 1, nestedUrl, token).ContinueWith(crawl =>
					{
						if (crawl.Result != null)
						{
							result.AddRange(crawl.Result);
						}
					});
				});

				await Task.WhenAll(crawlTasks);
			}

			return result;
		}
	}
}