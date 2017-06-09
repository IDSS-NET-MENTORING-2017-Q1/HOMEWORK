using System.Drawing;
using System.IO;
using ScannerManager.Interfaces;
using ZXing;

namespace ScannerManager.Classes
{
	public class BarcodeManager : IBarcodeManager
	{
		private string _endOfDocument = "EndOfDocument";

		public string EndOfDocument
		{
			get
			{
				return _endOfDocument;
			}
			set
			{
				_endOfDocument = value;
			}
		}

		public bool IsBarcode(string fileName)
		{
			if (string.IsNullOrWhiteSpace(fileName))
			{
				return false;
			}

			if (!File.Exists(fileName))
			{
				return false;
			}

			using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
			{
				using (var bitmap = Image.FromStream(fs) as Bitmap)
				{
					var reader = new BarcodeReader { AutoRotate = true };
					var decodeResult = reader.Decode(bitmap);

					if (decodeResult != null && decodeResult.Text == _endOfDocument)
					{
						return true;
					}
				}
			}

			return false;
		}
	}
}
