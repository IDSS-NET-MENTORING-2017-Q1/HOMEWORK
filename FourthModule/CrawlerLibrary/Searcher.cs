﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CrawlerLibrary
{
	public static class Searcher
	{
		private static IEnumerable<string> BreakPhrase(string phrase)
		{
			Regex specialCharacters = new Regex("[',.?!:;\"]{1}");
			return specialCharacters.Replace(phrase, " ").Split(' ');
		}

		public static bool FindPhrase(string source, string phrase)
		{
			IEnumerable<string> words = BreakPhrase(phrase);
			return source.Contains(phrase) || words.Any(word => source.IndexOf(word, StringComparison.OrdinalIgnoreCase) >= 0);
		}
	}
}
