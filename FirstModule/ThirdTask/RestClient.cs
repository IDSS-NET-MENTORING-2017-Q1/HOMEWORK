using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace ThirdTask
{
	public class RestClient
	{
		public TResult Get<TResult>(string url)
		{
			TResult result;
			
			ServicePointManager.ServerCertificateValidationCallback += (send, certificate, chain, sslPolicyErrors) => true;

			try
			{
				using (WebClient webClient = new WebClient())
				{
					webClient.Headers["User-Agent"] = "Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US; rv:1.9.2.6) Gecko/20100625 Firefox/3.6.6 (.NET CLR 3.5.30729)";

					var stream = webClient.OpenRead(new Uri(url));
					if (stream == null)
						throw new HttpException();
					
					using (StreamReader textReader = new StreamReader(stream))
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
				using (WebClient webClient = new WebClient())
				{
					webClient.Headers["User-Agent"] = "Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US; rv:1.9.2.6) Gecko/20100625 Firefox/3.6.6 (.NET CLR 3.5.30729)";

					var stream = await webClient.OpenReadTaskAsync(new Uri(url));
					if (stream == null)
						throw new HttpException();

					using (StreamReader textReader = new StreamReader(stream))
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
