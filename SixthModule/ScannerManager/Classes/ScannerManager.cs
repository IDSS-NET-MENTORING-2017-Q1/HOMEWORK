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
		private int _checkInterval;
		private Timer _checkTimer;

		private FileManagerFactory _fileManagerFactory;
		private ICollection<PathWatcher> _pathWatchers = new List<PathWatcher>();

		private BarcodeManager _barcodeManager;
		private DocumentManager _documentManager;

		private IReceiver<Settings> _settingsListener;

		public int CheckInterval
		{
			get
			{
				return _checkInterval;
			}
			set
			{
				_checkInterval = value;
			}
		}

		public IReceiver<Settings> SettingsReceiver
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

		public BarcodeManager BarcodeManager
		{
			get
			{
				return _barcodeManager;
			}
		}

		public DocumentManager DocumentManager
		{
			get
			{
				return _documentManager;
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
			_checkTimer = new Timer(_checkInterval)
			{
				Enabled = true,
				AutoReset = true
			};

			_checkTimer.Elapsed += CheckTimer_Elapsed;

			_fileManagerFactory = new FileManagerFactory(outputPath, tempPath, corruptedPath);
			_documentManager = new DocumentManager();
			_barcodeManager = new BarcodeManager();

			if (sourcePaths == null || !sourcePaths.Any(path => !IsUnc(path)))
			{
				var fileManager = _fileManagerFactory.Create(null);
				var pathWatcher = new PathWatcher(fileManager.InputPath)
				{
					FileManager = fileManager,
					DocumentManager = _documentManager,
					BarcodeManager = _barcodeManager
				};

				_pathWatchers.Add(pathWatcher);

				return;
			}

			foreach (var sourcePath in sourcePaths.Where(path => !IsUnc(path)))
			{
				var fileManager = _fileManagerFactory.Create(sourcePath);
				var pathWatcher = new PathWatcher(sourcePath) {
					FileManager = fileManager,
					DocumentManager = _documentManager,
					BarcodeManager = _barcodeManager
				};

				_pathWatchers.Add(pathWatcher);
			}
		}

		void CheckTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			var settings = _settingsListener.Receive();

			foreach (var pathWatcher in _pathWatchers)
			{
				pathWatcher.WaitInterval = settings.Timeout;
				pathWatcher.BarcodeManager.EndOfDocument = settings.EndOfDocument;
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

			return true;
		}
	}
}
