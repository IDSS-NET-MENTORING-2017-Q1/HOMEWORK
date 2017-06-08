using CustomMessaging.Unity;
using Scanner.Interfaces;
using System.Drawing;
using System.IO;
using ZXing;

namespace Scanner.Classes
{
	[LogFileName("barcode_manager_logs")]
	public class BarcodeManager : IBarcodeManager
	{
		private string _endOfDocument = "EndOfDocument";

		public BarcodeManager()
		{

		}

		public BarcodeManager(string endOfDocument)
		{
			_endOfDocument = endOfDocument;
		}

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

			using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
			{
				using (Bitmap bitmap = Bitmap.FromStream(fs) as Bitmap)
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
