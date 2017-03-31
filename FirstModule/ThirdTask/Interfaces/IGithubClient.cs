using System.Collections.Generic;
using ThirdTask.Classes;

namespace ThirdTask.Interfaces
{
	public interface IGithubClient
	{
		IEnumerable<GithubRepository> SearchRepositories(string queryString);
	}
}