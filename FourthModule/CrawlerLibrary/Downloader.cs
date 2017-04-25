using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace CrawlerLibrary
{
	public class Downloader
    {
		public string DownloadPage(string url)
		{
			WebClient client = new WebClient();
			string result = client.DownloadString(url);
			return result;
		}

		public Task<string> DownloadPageAsync(string url, CancellationToken token)
		{
			WebClient client = new WebClient();
			object lockObj = new object();
			TaskCompletionSource<string> taskSource = new TaskCompletionSource<string>();

			token.Register(() =>
			{
				client.CancelAsync();
			});

			client.DownloadStringCompleted += (sender, args) =>
			{
				if (args.Cancelled == true)
				{
					taskSource.TrySetCanceled();
					return;
				}
				else if (args.Error != null)
				{
					taskSource.TrySetException(args.Error);
					return;
				}
				else
				{
					lock (lockObj)
					{
						taskSource.TrySetResult(args.Result);
					}
				}
			};

			try
			{
				Uri sourceUri = new Uri(url);
				client.DownloadStringAsync(sourceUri);
			}
			catch (UriFormatException exception)
			{
				taskSource.TrySetException(exception);
				return taskSource.Task;
			}

			return taskSource.Task;
		}

	}
}
