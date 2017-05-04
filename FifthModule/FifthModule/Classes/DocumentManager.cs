using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.Rendering;

namespace FifthModule.Classes
{
	public class DocumentManager
	{
		private FileManager _fileManager;

		public FileManager FileManager
		{
			get
			{
				return _fileManager;
			}
			set
			{
				_fileManager = value;
			}
		}

		public DocumentManager(FileManager fileManager)
		{
			_fileManager = fileManager;
		}

		public void GeneratePdf() {
			if (_fileManager == null)
			{
				throw new InvalidOperationException("File manager is not defined!");
			}

			var document = new Document();
			var section = document.AddSection();

			foreach (string fileName in _fileManager.GetTempFiles())
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

			var renderer = new PdfDocumentRenderer();
			renderer.Document = document;
			renderer.RenderDocument();

			var resultName = Path.Combine(_fileManager.OutputPath, Guid.NewGuid().ToString());
			
			renderer.Save(resultName);

			_fileManager.ClearTemp();
		}
	}
}
