using System.Collections.Generic;
using NUnit.Framework;
using System.Linq;
using System.Net;
using Moq;
using ThirdTask.Classes;
using ThirdTask.Interfaces;

namespace ThirdTask.Tests
{
	[TestFixture]
	public class GithubQueryTests
	{
		[Test]
		public void RestClient_GetWithHeaders_ReturnsResponse()
		{
			// Arrange
			var restClient = new RestClient();
			restClient.Headers.Add("User-Agent",
				"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US; rv:1.9.2.6) Gecko/20100625 Firefox/3.6.6 (.NET CLR 3.5.30729)");

			// Act
			var result =
				restClient.Get<GithubResponse>(
					"https://api.github.com/search/repositories?q=tetris+language:assembly&sort=stars&order=desc");

			// Assert
			Assert.IsNotNull(result, "Result should not be null!");
			Assert.Greater(result.TotalCount, 0, "Result count should be greater than zero!");
		}

		[Test]
		public void RestClient_GetWithoutHeaders_ThrowsAnException()
		{
			// Arrange
			var restClient = new RestClient();

			// Assert
			Assert.Throws<WebException>(() => restClient.Get<GithubResponse>(
				"https://api.github.com/search/repositories?q=tetris+language:assembly&sort=stars&order=desc"));
		}

		[Test]
		public void GithubClient_SearchRepositories_FormsCorrectUrl()
		{
			// Arrange
			const int expectedCount = 3;
			const string expectedUrl =
				"https://api.github.com/search/repositories?q=tetris in:name+language:assembly&sort=stars&order=asc";

			var mockRestClient = new Mock<IRestClient>();
			mockRestClient.Setup(rc => rc.Get<GithubResponse>(expectedUrl)).Returns(
				new GithubResponse()
				{
					TotalCount = 3,
					Items = new List<GithubRepository>()
					{
						new GithubRepository()
						{
							Name = "Tetris #1",
							Url = "Tetris #1",
							Stars = 0,
							FullName = "Tetris #1",
							Id = 1,
							Language = "Assembly"
						},
						new GithubRepository()
						{
							Name = "Tetris #2",
							Url = "Tetris #2",
							Stars = 1,
							FullName = "Tetris #2",
							Id = 2,
							Language = "Assembly"
						},
						new GithubRepository()
						{
							Name = "Tetris #3",
							Url = "Tetris #3",
							Stars = 2,
							FullName = "Tetris #3",
							Id = 3,
							Language = "Assembly"
						}
					}
				}
			);

			var githubClient = new GithubClient(mockRestClient.Object);

			// Act
			var result = githubClient.SearchRepositories("q=tetris in:name+language:assembly&sort=stars&order=asc");

			// Assert
			Assert.IsNotNull(result, "Result should not be null!");
			Assert.AreEqual(expectedCount, result.Count(), "Result count should be equal to 3!");
		}

		[Test]
		public void GithubQuery_RepositoriesSearch_ReturnsRepositoriesList()
		{
			var exprectedQuery = "q=tetris in:name+language:assembly&sort=stars&order=asc";
			var mockGithubClient = new Mock<IGithubClient>();
			mockGithubClient.Setup(g => g.SearchRepositories(exprectedQuery)).Returns(new List<GithubRepository>()
			{
				new GithubRepository()
						{
							Name = "Tetris #1",
							Url = "Tetris #1",
							Stars = 0,
							FullName = "Tetris #1",
							Id = 1,
							Language = "Assembly"
						},
						new GithubRepository()
						{
							Name = "Tetris #2",
							Url = "Tetris #2",
							Stars = 1,
							FullName = "Tetris #2",
							Id = 2,
							Language = "Assembly"
						},
						new GithubRepository()
						{
							Name = "Tetris #3",
							Url = "Tetris #3",
							Stars = 2,
							FullName = "Tetris #3",
							Id = 3,
							Language = "Assembly"
						}
			});

			var provider = new GithubQueryProvider(mockGithubClient.Object);
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