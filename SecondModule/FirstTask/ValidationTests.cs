using System.IO;
using System.Reflection;
using Library.Validation;
using NUnit.Framework;

namespace FirstTask
{
	[TestFixture]
	public class ValidationTests
	{
		[Test]
		public void SuccessfulValidation()
		{
			var expected = true;
			var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			var filePath = basePath + "\\Samples\\Correct.xml";

			var validator = new Validator();
			validator.Schemas.Add("http://tempuri.org/Books.xsd", basePath + "\\Schemas\\Books.xsd");

			var result = validator.Validate(filePath);

			Assert.AreEqual(expected, result.Status, "This file should pass validation!");
		}

		[Test]
		public void FailedValidation()
		{
			var expected = false;
			var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			var filePath = basePath + "\\Samples\\Incorrect.xml";

			var validator = new Validator();
			validator.Schemas.Add("http://tempuri.org/Books.xsd", basePath + "\\Schemas\\Books.xsd");

			var result = validator.Validate(filePath);

			Assert.AreEqual(expected, result.Status, "This file should fail validation!");
		}
	}
}
