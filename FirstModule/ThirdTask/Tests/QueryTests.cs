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
	public class QueryTests
	{
		[Test]
		public void Get_ReturnsResponse()
		{
			var restClient = new RestClient();
			var result = restClient.Get<GithubResponse>("https://api.github.com/search/repositories?q=tetris+language:assembly&sort=stars&order=desc");
			Assert.IsNotNull(result, "Result should not be null!");
		}

		[Test]
		public void UsersSearch_ReturnsUsersList()
		{
			var restClient = new RestClient();
			var githubClient = new GithubClient(restClient);
			var provider = new GithubQueryProvider(githubClient);
			var users = new GithubQuery<GithubRepository>(provider);

			foreach (var user in users.Where(e => e.Name == "EPRUIZHW0249"))
			{
				Console.WriteLine("{0} {1}", user.FullName, user.Url);
			}
		}
	}
}
