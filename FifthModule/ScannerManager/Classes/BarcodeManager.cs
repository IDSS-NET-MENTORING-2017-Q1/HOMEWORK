using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

			var bitmap = Bitmap.FromFile(fileName) as Bitmap;

			var reader = new BarcodeReader { AutoRotate = true };
			var decodeResult = reader.Decode(bitmap);

			if (decodeResult != null)
			{
				return true;
			}

			return false;
		}
	}
}
