using System;
using System.Drawing;
using System.IO;
using CustomMessaging.Interfaces;
using CustomMessaging.Unity;
using ScannerManager.Interfaces;
using ZXing;

namespace ScannerManager.Classes
{
	[LogFileName("barcode_manager_logs")]
	public class BarcodeManager : IBarcodeManager, IIdentifiable
	{
		private string _endOfDocument = "EndOfDocument";
		private Guid _objectGuid = Guid.NewGuid();

		public string ObjectGuid
		{
			get
			{
				return _objectGuid.ToString();
			}
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
