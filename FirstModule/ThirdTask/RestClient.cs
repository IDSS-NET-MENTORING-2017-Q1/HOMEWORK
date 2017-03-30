using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Web;
using Newtonsoft.Json;

namespace ThirdTask
{
	public class RestClient
	{
		public TResult Get<TResult>(string url)
		{
			TResult result;
			var request = WebRequest.Create(new Uri(url));
			try
			{
				var responseStream = request.GetResponse().GetResponseStream();
				if (responseStream == null)
					throw new HttpException();

				using (var textReader = new StreamReader(responseStream))
				{
					using (var jsonReader = new JsonTextReader(textReader))
					{
						var serializer = new JsonSerializer();
						result = serializer.Deserialize<TResult>(jsonReader);
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
