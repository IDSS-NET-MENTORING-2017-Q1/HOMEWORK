using System;
using System.IO;
using Library.Transformation;

namespace SecondTask
{
	class Program
	{
		static void Main(string[] args)
		{
			string sourcePath = string.Empty;
			while (!File.Exists(sourcePath))
			{
				Console.WriteLine("Paste path to your xml here:");
				sourcePath = Console.ReadLine();

				if (!File.Exists(sourcePath))
					Console.WriteLine("There is no such xml!");
			}

			string stylesheetPath = "Transformation/RSS.xslt";

			Console.WriteLine("Paste path to your destination here:");
			var destinationPath = Console.ReadLine();

			var transormator = new Transformator(stylesheetPath);
			transormator.Transform(sourcePath, destinationPath);
		}
	}
}
