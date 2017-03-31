using System.Collections.Generic;
using System.Linq;
using ThirdTask.Interfaces;

namespace ThirdTask.Classes
{
	public class GithubClient : IGithubClient
	{
		private readonly string _baseUrl = "https://api.github.com";
		private readonly string _repositoriesUrl = "/search/repositories";
		private readonly IRestClient _restClient;

		public string BaseUrl
		{
			get { return _baseUrl; }
		}

		public string RepositoriesUrl
		{
			get { return _repositoriesUrl; }
		}

		public GithubClient(IRestClient restClient)
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