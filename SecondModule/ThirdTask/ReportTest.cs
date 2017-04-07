using NUnit.Framework;
using System.IO;
using Library.Transformation;
using System.Reflection;

namespace ThirdTask
{
	[TestFixture]
	public class ReportTest
	{
		[Test]
		public void TestMethod()
		{
			var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			string sourcePath = basePath + "\\Samples\\Correct.xml";
			string stylesheetPath = basePath + "\\Transformation\\Report.xslt";
			string destinationPath = basePath + "\\Samples\\Report.html";

			if (File.Exists(destinationPath))
				File.Delete(destinationPath);

			var transormator = new Transformator(stylesheetPath);
			transormator.Transform(sourcePath, destinationPath);

			var expected = true;
			var actual = File.Exists(destinationPath);
			Assert.AreEqual(expected, actual, "Report.html file must be created by this test!");
		}
	}
}
