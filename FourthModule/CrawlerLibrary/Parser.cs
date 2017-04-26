using HtmlAgilityPack;
using System.Collections.Generic;
using System.Xml.XPath;

namespace CrawlerLibrary
{
	public static class Parser
	{
		public static IEnumerable<string> GetUrls(string xml)
		{
			List<string> urls = new List<string>();
			HtmlDocument document = new HtmlDocument();
			document.LoadHtml(xml);
			XPathNavigator navigator = document.CreateNavigator();
			XPathNodeIterator iterator = navigator.Select("//@href");
			while (iterator.MoveNext())
			{
				XPathNavigator item = iterator.Current;
				urls.Add(item.Value);
			}
			return urls;
		}
	}
}
