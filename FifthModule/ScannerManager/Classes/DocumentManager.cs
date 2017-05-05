using System.Collections.Generic;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.Rendering;

namespace Scanner.Classes
{
	public class DocumentManager
	{
		public void GeneratePdf(string destination, IEnumerable<string> sourceFiles) {
			var document = new Document();
			var section = document.AddSection();

			foreach (string fileName in sourceFiles)
			{
				var image = section.AddImage(fileName);

				image.RelativeHorizontal = RelativeHorizontal.Page;
				image.RelativeVertical = RelativeVertical.Page;

				image.Top = 0;
				image.Left = 0;

				image.Height = document.DefaultPageSetup.PageHeight;
				image.Width = document.DefaultPageSetup.PageWidth;

				section.AddPageBreak();
			}

			var renderer = new PdfDocumentRenderer()
			{
				Document = document
			};

			renderer.PrepareRenderPages();
			//renderer.RenderDocument();
			renderer.Save(destination);
		}
	}
}
