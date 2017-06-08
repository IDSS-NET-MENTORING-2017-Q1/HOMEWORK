using System.Collections.Generic;

using CustomMessaging.DTO;
using CustomMessaging.Interfaces;
using Scanner.Interfaces;
using CustomMessaging.Unity;
using System;
using Microsoft.Practices.Unity;

namespace Scanner.Classes
{
	[LogFileName("scanner_manager_logs")]
	public class ScannerManager : IScannerManager, IIdentifiable
	{
		private ICollection<IPathWatcher> _pathWatchers = new List<IPathWatcher>();
		private Guid _objectGuid = Guid.NewGuid();

		public string ObjectGuid
		{
			get
			{
				return _objectGuid.ToString();
			}
		}

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

		public ICollection<IPathWatcher> PathWatchers
		{
			get
			{
				return _pathWatchers;
			}
		}

		protected void Init(IEnumerable<IFileManager> fileManagers, IUnityContainer container)
		{
			_settingsListener = container.Resolve<IListener<SettingsDTO>>();
			_settingsListener.Received += SettingsListener_Received;

			foreach (var fileManager in fileManagers)
			{				
				var pathWatcher = container.Resolve<IPathWatcher>(new ParameterOverride("inputPath", fileManager.InputPath));
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

		public ScannerManager(IEnumerable<IFileManager> fileManagers, IUnityContainer container)
		{
			Init(fileManagers, container);
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
