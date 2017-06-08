using System.Collections.Generic;

using CustomMessaging.DTO;
using CustomMessaging.Interfaces;
using Scanner.Interfaces;
using CustomMessaging.Unity;

namespace Scanner.Classes
{
	[LogFileName("scanner_manager_logs")]
	public class ScannerManager
	{
		private ICollection<PathWatcher> _pathWatchers = new List<PathWatcher>();
		
		private IListener<SettingsDTO> _settingsListener;
		
		public IListener<SettingsDTO> SettingsListener
		{
			get
			{
				return _settingsListener;
			}
			set
			{
				_settingsListener = value;
			}
		}

		public ICollection<PathWatcher> PathWatchers
		{
			get
			{
				return _pathWatchers;
			}
		}

		protected void Init(IEnumerable<IFileManager> fileManagers, IDocumentManager documentManager, IBarcodeManager barcodeManager, IPublisher<IEnumerable<byte>> documentPublisher, IPublisher<StatusDTO> statusPublisher, IListener<SettingsDTO> settingsListener)
		{
			_settingsListener = settingsListener;
			_settingsListener.Received += SettingsListener_Received;

			foreach (var fileManager in fileManagers)
			{
				var pathWatcher = new PathWatcher(fileManager.InputPath)
				{
					FileManager = fileManager,
					DocumentManager = documentManager,
					BarcodeManager = barcodeManager,
					DocumentPublisher = documentPublisher,
					StatusPublisher = statusPublisher
				};

				_pathWatchers.Add(pathWatcher);
			}
		}

		void SettingsListener_Received(object sender, SettingsDTO e)
		{
			foreach (var pathWatcher in _pathWatchers)
			{
				pathWatcher.WaitInterval = e.Timeout;
				pathWatcher.BarcodeManager.EndOfDocument = e.EndOfDocument;
			}
		}

		public ScannerManager(IEnumerable<IFileManager> fileManagers, IDocumentManager documentManager, IBarcodeManager barcodeManager, IPublisher<IEnumerable<byte>> documentPublisher, IPublisher<StatusDTO> statusPublisher, IListener<SettingsDTO> settingsListener)
		{
			Init(fileManagers, documentManager, barcodeManager, documentPublisher, statusPublisher, settingsListener);
		}

		public bool Start()
		{
			_settingsListener.Start();

			foreach (var pathWatcher in _pathWatchers)
			{
				pathWatcher.Start();
			}

			return true;
		}

		public bool Stop()
		{
			foreach (var pathWatcher in _pathWatchers)
			{
				pathWatcher.Stop();
			}

			_settingsListener.Dispose();

			return true;
		}
	}
}
