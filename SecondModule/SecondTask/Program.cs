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

			string stylesheetPath = string.Empty;
			while (!File.Exists(stylesheetPath))
			{
				Console.WriteLine("Paste path to your stylesheet here:");
				stylesheetPath = Console.ReadLine();

				if (!File.Exists(stylesheetPath))
					Console.WriteLine("There is no such stylesheet!");
			}

			Console.WriteLine("Paste path to your destination here:");
			var destinationPath = Console.ReadLine();

			var transormator = new Transformator(stylesheetPath);
			transormator.Transform(sourcePath, destinationPath);
		}
	}
}
