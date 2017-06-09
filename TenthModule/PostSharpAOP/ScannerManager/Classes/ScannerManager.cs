using System.Collections.Generic;
using CustomMessaging.Aspects;
using CustomMessaging.DTO;
using CustomMessaging.Interfaces;

namespace ScannerManager.Classes
{
	public class ScannerManager
	{
		private readonly ICollection<PathWatcher> _pathWatchers = new List<PathWatcher>();
		
		private IListener<SettingsDto> _settingsListener;
		
		public IListener<SettingsDto> SettingsListener
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

		[LogMethod]
		protected void Init(IEnumerable<FileManager> fileManagers, DocumentManager documentManager, BarcodeManager barcodeManager, IPublisher<IEnumerable<byte>> documentPublisher, IPublisher<StatusDto> statusPublisher, IListener<SettingsDto> settingsListener)
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

		[LogMethod]
		private void SettingsListener_Received(object sender, SettingsDto e)
		{
			foreach (var pathWatcher in _pathWatchers)
			{
				pathWatcher.WaitInterval = e.Timeout;
				pathWatcher.BarcodeManager.EndOfDocument = e.EndOfDocument;
			}
		}

		public ScannerManager(IEnumerable<FileManager> fileManagers, DocumentManager documentManager, BarcodeManager barcodeManager, IPublisher<IEnumerable<byte>> documentPublisher, IPublisher<StatusDto> statusPublisher, IListener<SettingsDto> settingsListener)
		{
			Init(fileManagers, documentManager, barcodeManager, documentPublisher, statusPublisher, settingsListener);
		}

		[LogMethod]
		public bool Start()
		{
			_settingsListener.Start();

			foreach (var pathWatcher in _pathWatchers)
			{
				pathWatcher.Start();
			}

			return true;
		}

		[LogMethod]
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
