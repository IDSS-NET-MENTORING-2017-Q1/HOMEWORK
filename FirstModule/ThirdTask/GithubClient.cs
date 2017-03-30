using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace ThirdTask
{
	public class GithubClient
	{
		private readonly string _baseUrl = "https://api.github.com";
		private readonly string _usersSearchUrl = "/search/repositories";
		private readonly RestClient _restClient;

		public string BaseUrl
		{
			get { return _baseUrl; }
		}

		public string UsersSearchUrl
		{
			get { return _usersSearchUrl; }
		}

		public GithubClient(RestClient restClient)
		{
			_restClient = restClient;
		}

		public IEnumerable<GithubRepository> SearchRepositories(string queryString)
		{
			var url = _baseUrl + _usersSearchUrl + "?" + queryString;
			return _restClient.Get<GithubResponse>(url).Items.Select(o => o);
		}
	}
}
