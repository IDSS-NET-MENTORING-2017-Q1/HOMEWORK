using NUnit.Framework;
using System.Linq;
using ThirdTask.Classes;

namespace ThirdTask.Tests
{
	[TestFixture]
	public class GithubQueryTests
	{
		[Test]
		public void RestClient_Get_ReturnsResponse()
		{
			var restClient = new RestClient();
			var result =
				restClient.Get<GithubResponse>(
					"https://api.github.com/search/repositories?q=tetris+language:assembly&sort=stars&order=desc");
			Assert.IsNotNull(result, "Result should not be null!");
			Assert.Greater(result.TotalCount, 0, "Result count should be greater than zero!");
		}

		[Test]
		public void GithubQuery_RepositoriesSearch_ReturnsRepositoriesList()
		{
			var restClient = new RestClient();
			var githubClient = new GithubClient(restClient);
			var provider = new GithubQueryProvider(githubClient);
			var repositories = new GithubQuery<GithubRepository>(provider);

			var filteredRepositories =
				repositories.Where(e => e.Name == "tetris" && e.Language == "assembly").OrderBy(e => e.Stars).ToList();

			Assert.IsNotNull(filteredRepositories, "Filtered list should not be null!");
			Assert.Greater(filteredRepositories.Count, 0, "Filtered list should not be empty!");
			for (int i = 0; i < filteredRepositories.Count - 1; i++)
			{
				Assert.GreaterOrEqual(filteredRepositories[i + 1].Stars, filteredRepositories[i].Stars,
					"Items should be ordered by stars!");
			}
		}
	}
}