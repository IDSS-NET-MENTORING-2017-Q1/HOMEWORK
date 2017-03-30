using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThirdTask.Attributes
{
	public class GithubFacetAttribute : Attribute
	{
		public string Prefix { get; set; }
	}
}
