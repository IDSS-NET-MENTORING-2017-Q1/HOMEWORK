using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
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

		private readonly ObservableCollection<string> _results = new ObservableCollection<string>();
		
		private Downloader _downloader;

		public int NestingLevel
		{
			get { return _nestingLevel; }
			set { _nestingLevel = value; }
		}

		public string SearchPhrase
		{
			get
			{
				return _searchPhrase;
			}

			set
			{
				_searchPhrase = value;
			}
		}

		public IEnumerable<string> BlockedExtensions
		{
			get
			{
				return _blockedExtensions;
			}

			set
			{
				_blockedExtensions = value;
			}
		}

		public Downloader Downloader
		{
			get
			{
				return _downloader;
			}

			set
			{
				_downloader = value;
			}
		}

		public ObservableCollection<string> Results
		{
			get
			{
				return _results;
			}
		}

		protected static void HandleCertificates()
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

		protected static bool ValidateUrl(string url)
		{
			Uri uriResult;
			return Uri.TryCreate(url, UriKind.Absolute, out uriResult)
				&& (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
		}

		public Crawler()
		{
			HandleCertificates();
		}

		public Crawler(Downloader downloader) : this()
		{
			_downloader = downloader;
		}

		public async Task CrawlAsync(Uri url, bool configureAwait, CancellationToken token)
		{
			_results.Clear();
			await CrawlAsync(0, url, configureAwait, token).ConfigureAwait(configureAwait);
		}

		public async Task CrawlAsync(string url, bool configureAwait, CancellationToken token)
		{
			Uri uri = new Uri(url);
			await CrawlAsync(uri, configureAwait, token).ConfigureAwait(configureAwait);
		}

		public async Task CrawlAsync(int level, string url, bool configureAwait, CancellationToken token)
		{
			Uri uri = new Uri(url);
			await CrawlAsync(level, uri, configureAwait, token).ConfigureAwait(configureAwait);
		}

		public async Task CrawlAsync(int level, Uri url, bool configureAwait, CancellationToken token)
		{
			string pageMarkup;
			try
			{
				pageMarkup = await _downloader.DownloadPageAsync(url).ConfigureAwait(configureAwait);
			}
			catch (HttpRequestException ex)
			{
				Debug.WriteLine(ex.Message);
				return;
			}

			token.ThrowIfCancellationRequested();

			if (Searcher.FindPhrase(pageMarkup, _searchPhrase))
			{
				_results.Add(url.AbsoluteUri);
			}

			token.ThrowIfCancellationRequested();

			if (level + 1 <= _nestingLevel)
			{
				IEnumerable<string> nestedUrls = Parser.GetUrls(pageMarkup).AsParallel()
					.Where(parsedUrl => {
						return ValidateUrl(parsedUrl) && !_blockedExtensions.Any(g => parsedUrl.EndsWith(g));
					}); ;
				
				foreach (string nestedUrl in nestedUrls)
				{
					token.ThrowIfCancellationRequested();
					await CrawlAsync(level + 1, nestedUrl, configureAwait, token).ConfigureAwait(configureAwait);
				}
			}
		}
	}
}
