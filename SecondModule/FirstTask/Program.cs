using System;
using Library.Validation;
using System.IO;

namespace FirstTask
{
	class Program
	{
		static void Main(string[] args)
		{
			string filePath = string.Empty;
			while (!File.Exists(filePath))
			{
				Console.WriteLine("Paste path to your xml here:");
				filePath = Console.ReadLine();

				if (!File.Exists(filePath))
					Console.WriteLine("There is no such xml!");
			}

			var validator = new Validator();
			validator.Schemas.Add("http://tempuri.org/Books.xsd", "Schemas/Books.xsd");

			var result = validator.Validate(filePath);

			if (result.Status)
			{
				Console.WriteLine("Everything is OK!");
			}
			else
			{
				Console.WriteLine("You have some errors in your file!");
				foreach (var error in result.Errors)
				{
					Console.WriteLine("Line number is {0}. Line position: {1}",
						error.LineNumber, error.LinePosition);
					Console.WriteLine(error.Message);
				}
			}

			Console.ReadLine();
		}
	}
}