using System.Drawing;
using System.IO;
using ZXing;

namespace FifthModule.Classes
{
	public class BarcodeManager
	{
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

					if (decodeResult != null)
					{
						return true;
					}
				}
			}

			return false;
		}
	}
}
