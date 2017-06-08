using System.Collections.Generic;
using System.Drawing;
using System.IO;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using Scanner.Interfaces;
using CustomMessaging.Unity;
using CustomMessaging.Interfaces;

namespace Scanner.Classes
{
	[LogFileName("document_manager_logs")]
	public class DocumentManager : IDocumentManager, IIdentifiable
	{
		private Guid _objectGuid = Guid.NewGuid();

		public string ObjectGuid
		{
			get
			{
				return _objectGuid.ToString();
			}
		}

		protected bool ValidateName(string fileName)
		{
			Guid guid;
			var value = Path.GetFileNameWithoutExtension(fileName);
			bool result = Guid.TryParse(value, out guid);
			return result;
		}

		public MemoryStream GeneratePdf(IEnumerable<string> sourceFiles)
		{
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
