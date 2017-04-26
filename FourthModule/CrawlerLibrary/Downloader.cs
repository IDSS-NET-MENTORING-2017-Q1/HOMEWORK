using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace CrawlerLibrary
{
	public class Downloader
    {
		private readonly Dictionary<string, string> _headers = new Dictionary<string, string>();

		public Dictionary<string, string> Headers => _headers;

		public string DownloadPage(string url)
		{
			WebClient client = new WebClient();
			string result = client.DownloadString(url);
			return result;
		}

		public async Task<string> DownloadPageAsync(string url)
		{
			HttpClient client = new HttpClient();

			foreach (var header in _headers)
			{
				client.DefaultRequestHeaders.Add(header.Key, header.Value);
			}

			return await client.GetStringAsync(url).ConfigureAwait(false);
		}

	}
}
