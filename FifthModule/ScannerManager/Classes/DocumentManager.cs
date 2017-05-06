using System.Collections.Generic;
using System.Drawing;
using System.IO;
using MigraDoc.Rendering;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace Scanner.Classes
{
	public class DocumentManager
	{
		public void GeneratePdf(string destination, IEnumerable<string> sourceFiles) {
			var document = new PdfDocument();

			foreach (string fileName in sourceFiles)
			{
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
