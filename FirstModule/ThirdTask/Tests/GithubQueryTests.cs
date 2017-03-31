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
			var client = new RestClient();
			client.Headers.Add("User-Agent",
				"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US; rv:1.9.2.6) Gecko/20100625 Firefox/3.6.6 (.NET CLR 3.5.30729)");

			// Act
			var result =
				client.Get<GithubResponse>(
					"https://api.github.com/search/repositories?q=tetris+language:assembly&sort=stars&order=desc");

			// Assert
			Assert.IsNotNull(result, "Result should not be null!");
			Assert.Greater(result.TotalCount, 0, "Result count should be greater than zero!");
		}

		[Test]
		public void RestClient_GetWithoutHeaders_ThrowsAnException()
		{
			// Arrange
			var client = new RestClient();

			// Assert
			Assert.Throws<WebException>(() => client.Get<GithubResponse>(
				"https://api.github.com/search/repositories?q=tetris+language:assembly&sort=stars&order=desc"));
		}

		[Test]
		public void GithubClient_SearchRepositories_FormsCorrectUrl()
		{
			// Arrange
			const int expectedCount = 3;
			const string expectedUrl =
				"https://api.github.com/search/repositories?q=tetris in:name+language:assembly&sort=stars&order=asc";

			var mockRest = new Mock<IRestClient>();
			mockRest.Setup(rc => rc.Get<GithubResponse>(expectedUrl)).Returns(
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

			var client = new GithubClient(mockRest.Object);

			// Act
			var result = client.SearchRepositories("q=tetris in:name+language:assembly&sort=stars&order=asc");

			// Assert
			Assert.IsNotNull(result, "Result should not be null!");
			Assert.AreEqual(expectedCount, result.Count(), "Result count should be equal to 3!");
		}

		[Test]
		public void GithubQuery_RepositoriesSearch_ReturnsRepositoriesList()
		{
			// Arrange
			const string expectedQuery = "q=tetris in:name+language:assembly&sort=stars&order=asc";
			var mockGithub = new Mock<IGithubClient>();
			mockGithub.Setup(g => g.SearchRepositories(expectedQuery)).Returns(new List<GithubRepository>()
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

			var provider = new GithubQueryProvider(mockGithub.Object);
			var repositories = new GithubQuery<GithubRepository>(provider);

			// Act
			var filteredRepositories =
				repositories.Where(e => e.Name == "tetris" && e.Language == "assembly").OrderBy(e => e.Stars).ToList();

			// Assert
			Assert.IsNotNull(filteredRepositories, "Filtered list should not be null!");
			Assert.Greater(filteredRepositories.Count, 0, "Filtered list should not be empty!");
			for (var i = 0; i < filteredRepositories.Count - 1; i++)
			{
				Assert.GreaterOrEqual(filteredRepositories[i + 1].Stars, filteredRepositories[i].Stars,
					"Items should be ordered by stars!");
			}
		}
	}
}