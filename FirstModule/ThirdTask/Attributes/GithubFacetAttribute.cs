using System;

namespace ThirdTask.Attributes
{
	public class GithubFacetAttribute : Attribute
	{
		public string Pattern { get; set; }
	}
}