using System.Collections.Generic;
using System.Drawing;
using System.IO;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;

namespace Scanner.Classes
{
	public class DocumentManager
	{
		protected bool ValidateName(string fileName)
		{
			Guid guid;
			var value = Path.GetFileName(fileName);
			bool result = Guid.TryParse(value, out guid);
			return result;
		}

		public void GeneratePdf(string destination, IEnumerable<string> sourceFiles) {
			var document = new PdfDocument();

			foreach (string fileName in sourceFiles)
			{
				if (!ValidateName(fileName))
				{
					continue;
				}

				var page = document.AddPage();
				var graphics = XGraphics.FromPdfPage(page);

				using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
				{
					using (Image image = Image.FromStream(fs))
					{
						var xImage = XImage.FromGdiPlusImage(image);
						graphics.DrawImage(xImage, 0, 0, page.Width, page.Height);
					}
				}
			}

			document.Save(destination);
		}
	}
}
