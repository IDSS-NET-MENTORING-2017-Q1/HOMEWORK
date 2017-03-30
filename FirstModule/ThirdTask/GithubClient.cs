using System.Collections.Generic;
using System.Linq;

namespace ThirdTask
{
	public class GithubClient
	{
		private readonly string _baseUrl = "https://api.github.com";
		private readonly string _repositoriesUrl = "/search/repositories";
		private readonly RestClient _restClient;

		public string BaseUrl
		{
			get { return _baseUrl; }
		}

		public string RepositoriesUrl
		{
			get { return _repositoriesUrl; }
		}

		public GithubClient(RestClient restClient)
		{
			_restClient = restClient;
		}

		public IEnumerable<GithubRepository> SearchRepositories(string queryString)
		{
			var url = _baseUrl + _repositoriesUrl + "?" + queryString;
			return _restClient.Get<GithubResponse>(url).Items.Select(o => o);
		}
	}
}
