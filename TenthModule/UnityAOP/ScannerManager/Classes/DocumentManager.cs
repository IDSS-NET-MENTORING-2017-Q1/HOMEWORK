using System.Collections.Generic;
using System.Drawing;
using System.IO;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using Scanner.Interfaces;
using CustomMessaging.Unity;
using CustomMessaging.Interfaces;
using System.Diagnostics;

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
