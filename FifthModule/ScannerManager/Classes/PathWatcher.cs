using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Scanner.Classes
{
	public class PathWatcher
	{
		private ManualResetEvent _stopRequested = new ManualResetEvent(false);
		private AutoResetEvent _fileCreated = new AutoResetEvent(false);
		private FileSystemWatcher _watcher;
		private Thread _worker;

		private FileManager _fileManager;
		private BarcodeManager _barcodeManager;
		private DocumentManager _documentManager;

		public PathWatcher(string inputPath)
		{
			_watcher = new FileSystemWatcher(inputPath);
			_watcher.Created += Watcher_Created;
			_worker = new Thread(WorkProcess);
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

					if (_fileManager.TryOpen(fileName, 3, 5000))
					{
						try
						{
							if (_barcodeManager.IsBarcode(fileName))
							{
								var sourceFiles = _fileManager.GetTempFiles();
								var pdfName = Path.Combine(_fileManager.OutputPath, Guid.NewGuid().ToString() + ".pdf");

								Debug.WriteLine(string.Format("Generating resulting PDF...", fileName));

								_fileManager.Delete(fileName);
								_documentManager.GeneratePdf(pdfName, sourceFiles);
								_fileManager.ClearTemp();
							}
							else
							{
								Debug.WriteLine(string.Format("Moving {0} to temp folder...", fileName));
								_fileManager.MoveToTemp(fileName);
							}

							continue;
						}
						catch (Exception ex)
						{
							Debug.WriteLine("Exception has occured!");
							Debug.WriteLine(ex.Message);
						}

						try
						{
							Debug.WriteLine(string.Format("Moving {0} to corrupted folder...", fileName));
							_fileManager.MoveToCorrupted(fileName);
						}
						catch (Exception ex)
						{
							Debug.WriteLine("Exception has occured!");
							Debug.WriteLine(ex.Message);
						}
					}
				}
			} while (WaitHandle.WaitAny(new WaitHandle[] { _stopRequested, _fileCreated }) != 0);
		}

		private void Watcher_Created(object sender, FileSystemEventArgs e)
		{
			_fileCreated.Set();
		}

		public void Start()
		{
			_worker.Start();
			_watcher.EnableRaisingEvents = true;
		}

		public void Stop()
		{
			_watcher.EnableRaisingEvents = false;
			_stopRequested.Set();
			_worker.Join();
		}

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
	}
}
