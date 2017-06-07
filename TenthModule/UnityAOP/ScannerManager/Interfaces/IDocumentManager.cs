using System.Collections.Generic;
using System.IO;

namespace Scanner.Interfaces
{
	public interface IDocumentManager
	{
		MemoryStream GeneratePdf(IEnumerable<string> sourceFiles);
	}
}