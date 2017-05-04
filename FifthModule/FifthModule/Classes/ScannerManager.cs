using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Topshelf;

namespace FifthModule.Classes
{
	public class ScannerManager : ServiceControl
	{
		private FileManager _fileManager;
		private BarcodeManager _barcodeManager;
		private DocumentManager _documentManager;

		private FileSystemWatcher _watcher;
		private Thread _worker;

		ManualResetEvent _stopRequested;
		AutoResetEvent _fileCreated;

		public FileManager FileManager
		{
			get
			{
				return _fileManager;
			}
			set
			{
				_fileManager = value;
			}
		}

		private void WorkProcess()
		{
			do
			{
				foreach (var fileName in _fileManager.GetInputFiles())
				{
					if (_stopRequested.WaitOne(TimeSpan.Zero))
					{
						return;
					}

					if (_fileManager.TryOpen(fileName, 3, 3000))
					{
						try
						{
							if (_barcodeManager.IsBarcode(fileName))
							{
								_fileManager.Delete(fileName);
								_documentManager.GeneratePdf();
								_fileManager.ClearTemp();
							}
							else
							{
								_fileManager.MoveToTemp(fileName);
							}
						}
						catch (Exception ex)
						{
							_fileManager.MoveToCorrupted(fileName);
						}						
					}
				}
			} while (WaitHandle.WaitAny(new WaitHandle[] {_stopRequested, _fileCreated}) != 0);
		}

		protected void Init()
		{
			Init(null, null, null, null);
		}

		protected void Init(string inputPath, string outputPath, string tempPath, string corruptedPath)
		{
			_fileManager = new FileManager(inputPath, outputPath, tempPath, corruptedPath);
			_documentManager = new DocumentManager(_fileManager);
			_barcodeManager = new BarcodeManager();

			CreateWatcherAndEvents();
		}

		protected void CreateWatcherAndEvents()
		{
			_watcher = new FileSystemWatcher(_fileManager.InputPath);
			_watcher.Created += Watcher_Created;
			_worker = new Thread(WorkProcess);

			_stopRequested = new ManualResetEvent(false);
			_fileCreated = new AutoResetEvent(false);
		}

		public ScannerManager()
		{
			Init();
		}

		public ScannerManager(FileManager fileManager, BarcodeManager barcodeManager, DocumentManager documentManager)
		{
			_fileManager = fileManager;
			_barcodeManager = barcodeManager;
			_documentManager = documentManager;

			CreateWatcherAndEvents();
		}

		public ScannerManager(string inputPath, string outputPath) : this(inputPath, outputPath, null, null) { }

		public ScannerManager(string inputPath, string outputPath, string tempPath, string corruptedPath)
		{
			Init(inputPath, outputPath, tempPath, corruptedPath);
		}

		private void Watcher_Created(object sender, FileSystemEventArgs e)
		{
			_fileCreated.Set();
		}

		public bool Start(HostControl hostControl)
		{
			_worker.Start();
			_watcher.EnableRaisingEvents = true;

			return true;
		}

		public bool Stop(HostControl hostControl)
		{
			_watcher.EnableRaisingEvents = false;
			_stopRequested.Set();
			_worker.Join();

			return true;
		}
	}
}
