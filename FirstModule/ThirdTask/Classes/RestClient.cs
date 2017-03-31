using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using ThirdTask.Interfaces;

namespace ThirdTask.Classes
{
	public class RestClient : IRestClient
	{
		private readonly Dictionary<string, string> _headers = new Dictionary<string, string>();

		public Dictionary<string, string> Headers
		{
			get { return _headers; }
		}

		public TResult Get<TResult>(string url)
		{
			TResult result;

			ServicePointManager.ServerCertificateValidationCallback += (send, certificate, chain, sslPolicyErrors) => true;

			try
			{
				using (var webClient = new WebClient())
				{
					foreach (var header in _headers)
					{
						webClient.Headers[header.Key] = header.Value;
					}

					var stream = webClient.OpenRead(new Uri(url));
					if (stream == null)
						throw new HttpException();

					using (var textReader = new StreamReader(stream))
					{
						var response = textReader.ReadToEnd();
						result = JsonConvert.DeserializeObject<TResult>(response);
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}

			return result;
		}

		public async Task<TResult> GetAsync<TResult>(string url)
		{
			TResult result;

			ServicePointManager.ServerCertificateValidationCallback += (send, certificate, chain, sslPolicyErrors) => true;

			try
			{
				using (var webClient = new WebClient())
				{
					foreach (var header in _headers)
					{
						webClient.Headers[header.Key] = header.Value;
					}

					var stream = await webClient.OpenReadTaskAsync(new Uri(url));
					if (stream == null)
						throw new HttpException();

					using (var textReader = new StreamReader(stream))
					{
						var response = await textReader.ReadToEndAsync();
						result = JsonConvert.DeserializeObject<TResult>(response);
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}

			return result;
		}
	}
}