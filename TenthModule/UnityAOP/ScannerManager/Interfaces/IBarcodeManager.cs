﻿namespace Scanner.Interfaces
{
	public interface IBarcodeManager
	{
		string EndOfDocument { get; set; }

		bool IsBarcode(string fileName);
	}
}