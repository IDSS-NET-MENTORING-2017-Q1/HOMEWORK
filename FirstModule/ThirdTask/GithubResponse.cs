using System.Collections.Generic;
using Newtonsoft.Json;

namespace ThirdTask
{
	[JsonObject]
	public class GithubResponse
	{
		[JsonProperty("total_count")]
		public int TotalCount { get; set; }
		[JsonProperty("items")]
		public IEnumerable<GithubRepository> Items { get; set; }
	}
}
