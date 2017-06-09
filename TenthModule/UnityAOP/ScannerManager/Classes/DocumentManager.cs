using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using ScannerManager.Interfaces;

namespace ScannerManager.Classes
{
	public class DocumentManager : IDocumentManager
	{
		protected bool ValidateName(string fileName)
		{
			Guid guid;
			var value = Path.GetFileNameWithoutExtension(fileName);
			var result = Guid.TryParse(value, out guid);
			return result;
		}

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
					Image image = null;
					try
					{
						image = Image.FromStream(fs);
						var xImage = XImage.FromGdiPlusImage(image);
						graphics.DrawImage(xImage, 0, 0, page.Width, page.Height);
					}
					catch (Exception ex)
					{
						Debug.WriteLine("Exception has occured!");
						Debug.WriteLine(ex.Message);
					} 
					finally
					{
						if (image != null)
						{
							image.Dispose();
						}
					}
				}
			}

			if (document.PageCount > 0)
			{
				var documentStream = new MemoryStream((int)document.FileSize);
				document.Save(documentStream);
				return documentStream;
			}

			return null;
		}
	}
}
