using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

using CustomMessaging.Classes;
using CustomMessaging.Interfaces;

namespace Scanner.Classes
{
	public class ScannerManager
	{
		private FileManagerFactory _fileManagerFactory;
		private ICollection<PathWatcher> _pathWatchers = new List<PathWatcher>();

		private IListener<Settings> _settingsListener;

		public IListener<Settings> SettingsReceiver
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

		public FileManagerFactory FileManagerFactory
		{
			get
			{
				return _fileManagerFactory;
			}
		}

		protected bool IsUnc(string sourcePath)
		{
			var uri = new Uri(sourcePath);
			return uri.IsUnc;
		}

		protected void Init()
		{
			Init(null, null, null, null);
		}

		protected void Init(IEnumerable<string> sourcePaths, string outputPath, string tempPath, string corruptedPath)
		{
			_settingsListener.Received += SettingsListener_Received;
			_fileManagerFactory = new FileManagerFactory(outputPath, tempPath, corruptedPath);

			var documentManager = new DocumentManager();
			var barcodeManager = new BarcodeManager();

			if (sourcePaths == null || !sourcePaths.Any(path => !IsUnc(path)))
			{
				var fileManager = _fileManagerFactory.Create(null);
				var pathWatcher = new PathWatcher(fileManager.InputPath)
				{
					FileManager = fileManager,
					DocumentManager = documentManager,
					BarcodeManager = barcodeManager
				};

				_pathWatchers.Add(pathWatcher);

				return;
			}

			foreach (var sourcePath in sourcePaths.Where(path => !IsUnc(path)))
			{
				var fileManager = _fileManagerFactory.Create(sourcePath);
				var pathWatcher = new PathWatcher(sourcePath)
				{
					FileManager = fileManager,
					DocumentManager = documentManager,
					BarcodeManager = barcodeManager
				};

				_pathWatchers.Add(pathWatcher);
			}
		}

		void SettingsListener_Received(object sender, Settings e)
		{
			foreach (var pathWatcher in _pathWatchers)
			{
				pathWatcher.WaitInterval = e.Timeout;
				pathWatcher.BarcodeManager.EndOfDocument = e.EndOfDocument;
			}
		}

		public ScannerManager()
		{
			Init();
		}

		public ScannerManager(IEnumerable<string> sourcesPath, string outputPath) : this(sourcesPath, outputPath, null, null) { }

		public ScannerManager(IEnumerable<string> sourcesPath, string outputPath, string tempPath, string corruptedPath)
		{
			Init(sourcesPath, outputPath, tempPath, corruptedPath);
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

			_settingsListener.Stop();

			return true;
		}
	}
}
