using Newtonsoft.Json;

namespace ThirdTask
{
	[JsonObject]
	public class GithubRepository
	{
		[JsonProperty("id")]
		public int Id { get; set; }
		[JsonProperty("name")]
		public string Name { get; set; }
		[JsonProperty("full_name")]
		public string FullName { get; set; }
		[JsonProperty("url")]
		public string Url { get; set; }
		[JsonProperty("language")]
		public string Language { get; set; }
	}
}
