using System;
using System.Collections.Generic;
using System.Linq;

namespace Scanner.Classes
{
	public class ScannerManager
	{
		private FileManagerFactory _fileManagerFactory;
		private ICollection<PathWatcher> _pathWatchers = new List<PathWatcher>();

		private BarcodeManager _barcodeManager;
		private DocumentManager _documentManager;

		public ICollection<PathWatcher> PathWatchers
		{
			get
			{
				return _pathWatchers;
			}
			set
			{
				_pathWatchers = value;
			}
		}

		public FileManagerFactory FileManagerFactory
		{
			get
			{
				return _fileManagerFactory;
			}
			set
			{
				_fileManagerFactory = value;
			}
		}

		public BarcodeManager BarcodeManager
		{
			get
			{
				return _barcodeManager;
			}
			set
			{
				_barcodeManager = value;
			}
		}

		public DocumentManager DocumentManager
		{
			get
			{
				return _documentManager;
			}
			set
			{
				_documentManager = value;
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
