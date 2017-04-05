using System;
using Library.Validation;

namespace FirstTask
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Paste path to your file here:");
			var filePath = Console.ReadLine();

			var validator = new Validator();
			validator.Schemas.Add("http://tempuri.org/Books.xsd", "Schemas/Books.xsd");

			var result = validator.Validate(filePath);

			if (result.Status)
			{
				Console.WriteLine("Everything is OK!");
			}
			else
			{
				Console.WriteLine("You have some errors in your file:");
				foreach (var validationError in result.Errors)
				{
					Console.WriteLine("Line number is {0}. Error message: {1}",
						validationError.LineNumber, validationError.Message);
				}
			}

			Console.ReadLine();
		}
	}
}