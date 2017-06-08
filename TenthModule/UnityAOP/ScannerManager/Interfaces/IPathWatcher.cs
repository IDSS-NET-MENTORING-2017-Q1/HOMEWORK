using System.Collections.Generic;
using CustomMessaging.DTO;
using CustomMessaging.Interfaces;
using Scanner.Interfaces;

namespace Scanner.Interfaces
{
	public interface IPathWatcher
	{
		IBarcodeManager BarcodeManager { get; set; }
		IDocumentManager DocumentManager { get; set; }
		IPublisher<IEnumerable<byte>> DocumentPublisher { get; set; }
		IFileManager FileManager { get; set; }
		int StatusInterval { get; set; }
		IPublisher<StatusDTO> StatusPublisher { get; set; }
		int WaitInterval { get; set; }

		void Start();
		void Stop();
	}
}