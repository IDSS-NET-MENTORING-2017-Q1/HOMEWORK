using System;
using System.Collections.Generic;
using System.Linq;

using CustomMessaging.Classes;
using CustomMessaging.Interfaces;

namespace Scanner.Classes
{
	public class ScannerManager
	{
		private ICollection<PathWatcher> _pathWatchers = new List<PathWatcher>();

		private FileManagerFactory _fileManagerFactory;
		private IListener<Settings> _settingsListener;
		
		public IListener<Settings> SettingsListener
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

		protected void Init(IEnumerable<string> sourcePaths, FileManagerFactory fileManagerFactory, DocumentManager documentManager, BarcodeManager barcodeManager, IPublisher<IEnumerable<byte>> documentPublisher, IPublisher<Status> statusPublisher, IListener<Settings> settingsListener)
		{
			_settingsListener = settingsListener;
			_settingsListener.Received += SettingsListener_Received;
			_fileManagerFactory = fileManagerFactory;

			if (sourcePaths == null || !sourcePaths.Any(path => !IsUnc(path)))
			{
				var fileManager = _fileManagerFactory.Create(null);
				var pathWatcher = new PathWatcher(fileManager.InputPath)
				{
					FileManager = fileManager,
					DocumentManager = documentManager,
					BarcodeManager = barcodeManager,
					DocumentPublisher = documentPublisher,
					StatusPublisher = statusPublisher
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
					BarcodeManager = barcodeManager,
					DocumentPublisher = documentPublisher,
					StatusPublisher = statusPublisher
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

		public ScannerManager(IEnumerable<string> sourcePaths, FileManagerFactory fileManagerFactory, DocumentManager documentManager, BarcodeManager barcodeManager, IPublisher<IEnumerable<byte>> documentPublisher, IPublisher<Status> statusPublisher, IListener<Settings> settingsListener)
		{
			Init(sourcePaths, fileManagerFactory, documentManager, barcodeManager, documentPublisher, statusPublisher, settingsListener);
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
