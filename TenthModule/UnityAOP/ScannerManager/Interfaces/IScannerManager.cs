using System.Collections.Generic;
using CustomMessaging.DTO;
using CustomMessaging.Interfaces;

namespace ScannerManager.Interfaces
{
	public interface IScannerManager
	{
		ICollection<IPathWatcher> PathWatchers { get; }
		IListener<SettingsDTO> SettingsListener { get; set; }

		bool Start();
		bool Stop();
	}
}