using System.Collections.Generic;
using CustomMessaging.DTO;
using CustomMessaging.Interfaces;
using Microsoft.Practices.Unity;
using ScannerManager.Interfaces;

namespace ScannerManager.Classes
{
	public class ScannerManager : IScannerManager
	{
		private readonly ICollection<IPathWatcher> _pathWatchers = new List<IPathWatcher>();

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
				var pathWatcher = container.Resolve<IPathWatcher>(
					new ParameterOverride("inputPath", fileManager.InputPath), 
					new ParameterOverride("fileManager", fileManager)
				);
				_pathWatchers.Add(pathWatcher);
			}
		}

		private void SettingsListener_Received(object sender, SettingsDTO e)
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
