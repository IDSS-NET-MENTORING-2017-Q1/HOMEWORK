using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ThirdTask.Tests
{
	[TestFixture]
	public class GithubQueryTests
	{
		[Test]
		public void RestClient_Get_ReturnsResponse()
		{
			var restClient = new RestClient();
			var result = restClient.Get<GithubResponse>("https://api.github.com/search/repositories?q=tetris+language:assembly&sort=stars&order=desc");
			Assert.IsNotNull(result, "Result should not be null!");
		}

		[Test]
		public void GithubQuery_RepositoriesSearch_ReturnsRepositoriesList()
		{
			var restClient = new RestClient();
			var githubClient = new GithubClient(restClient);
			var provider = new GithubQueryProvider(githubClient);
			var repositories = new GithubQuery<GithubRepository>(provider);


			foreach (var repository in repositories.Where(e => e.Name == "tetris" && e.Language == "assembly").OrderBy(e => e.Stars))
			{
				Console.WriteLine("{0} {1}", repository.FullName, repository.Url);
			}
		}
	}
}
