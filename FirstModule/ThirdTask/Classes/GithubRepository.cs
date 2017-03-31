using Newtonsoft.Json;
using ThirdTask.Attributes;

namespace ThirdTask.Classes
{
	[JsonObject]
	public class GithubRepository
	{
		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("name")]
		[GithubFacet(Pattern = "{value} in:name")]
		public string Name { get; set; }

		[JsonProperty("full_name")]
		[GithubFacet(Pattern = "{value} in:full_name")]
		public string FullName { get; set; }

		[JsonProperty("url")]
		[GithubFacet(Pattern = "{value} in:url")]
		public string Url { get; set; }

		[JsonProperty("language")]
		[GithubFacet(Pattern = "language:{value}")]
		public string Language { get; set; }

		[JsonProperty("stars")]
		[GithubFacet(Pattern = "stars:{value}")]
		[GithubSorter(Name = "stars")]
		public int Stars { get; set; }
	}
}