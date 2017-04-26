using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CrawlerLibrary
{
	public class Downloader
    {
		private readonly Dictionary<string, string> _headers = new Dictionary<string, string>();

		public Dictionary<string, string> Headers => _headers;

		public async Task<string> DownloadPageAsync(string url)
		{
			Uri uri = new Uri(url);
			return await DownloadPageAsync(uri).ConfigureAwait(false);
		}

		public async Task<string> DownloadPageAsync(Uri url)
		{
			using (HttpClient client = new HttpClient())
			{
				foreach (var header in _headers)
				{
					client.DefaultRequestHeaders.Add(header.Key, header.Value);
				}

				byte[] response = await client.GetByteArrayAsync(url).ConfigureAwait(false);

				return Encoding.UTF8.GetString(response);
			}
		}
	}
}
