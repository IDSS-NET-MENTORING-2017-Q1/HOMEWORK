using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using CustomMessaging.Aspects;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace ScannerManager.Classes
{
	public class DocumentManager
	{
		[LogMethod]
		protected bool ValidateName(string fileName)
		{
			Guid guid;
			var value = Path.GetFileNameWithoutExtension(fileName);
			var result = Guid.TryParse(value, out guid);
			return result;
		}

		[LogMethod]
		public MemoryStream GeneratePdf(IEnumerable<string> sourceFiles)
		{
			var document = new PdfDocument();

			foreach (var fileName in sourceFiles)
			{
				if (!ValidateName(fileName))
				{
					continue;
				}

				var page = document.AddPage();
				var graphics = XGraphics.FromPdfPage(page);

				using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
				{
					using (var image = Image.FromStream(fs))
					{
						var xImage = XImage.FromGdiPlusImage(image);
						graphics.DrawImage(xImage, 0, 0, page.Width, page.Height);
					}
				}
			}

			var result = new MemoryStream((int)document.FileSize);
			document.Save(result);
			return result;
		}
	}
}
