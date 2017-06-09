using System.Collections.Generic;
using System.IO;

namespace ScannerManager.Interfaces
{
	public interface IDocumentManager
	{
		MemoryStream GeneratePdf(IEnumerable<string> sourceFiles);
	}
}