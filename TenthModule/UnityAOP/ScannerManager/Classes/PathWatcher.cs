using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using CustomMessaging.DTO;
using CustomMessaging.Enums;
using CustomMessaging.Interfaces;
using ScannerManager.Interfaces;
using SystemTimer = System.Timers.Timer;

namespace ScannerManager.Classes
{
	public class PathWatcher : IPathWatcher
	{
		private readonly StatusDTO _status;
		
		private int _waitInterval = 10000;
		private int _statusInterval = 10000;

		private readonly SystemTimer _statusTimer;
		private readonly ManualResetEvent _stopRequested = new ManualResetEvent(false);
		private readonly AutoResetEvent _fileCreated = new AutoResetEvent(false);
		private readonly FileSystemWatcher _watcher;
		private readonly Thread _worker;

		private IFileManager _fileManager;
		private IBarcodeManager _barcodeManager;
		private IDocumentManager _documentManager;
		private IPublisher<IEnumerable<byte>> _documentPublisher;
		private IPublisher<StatusDTO> _statusPublisher;

		public int WaitInterval
		{
			get
			{
				return _waitInterval;
			}
			set
			{
				_waitInterval = value;
			}
		}

		public int StatusInterval
		{
			get
			{
				return _statusInterval;
			}
			set
			{
				_statusInterval = value;
			}
		}

		public IFileManager FileManager
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

		public IBarcodeManager BarcodeManager
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

		public IDocumentManager DocumentManager
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

		public IPublisher<IEnumerable<byte>> DocumentPublisher
		{
			get
			{
				return _documentPublisher;
			}
			set
			{
				_documentPublisher = value;
			}
		}

		public IPublisher<StatusDTO> StatusPublisher
		{
			get { return _statusPublisher; }
			set { _statusPublisher = value; }
		}

		public PathWatcher(string inputPath, IFileManager fileManager, IBarcodeManager barcodeManager, IDocumentManager documentManager, IPublisher<IEnumerable<byte>> documentPublisher, IPublisher<StatusDTO> statusPublisher)
		{
			_fileManager = fileManager;
			_barcodeManager = barcodeManager;
			_documentManager = documentManager;
			_documentPublisher = documentPublisher;
			_statusPublisher = statusPublisher;

			_status = new StatusDTO
			{
				Value = ServiceStatuses.Waiting,
				ServiceName = Guid.NewGuid().ToString()
			};

			_statusTimer = new SystemTimer(_statusInterval)
			{
				Enabled = false,
				AutoReset = true
			};

			_watcher = new FileSystemWatcher(inputPath);
			_worker = new Thread(WorkProcess);

			_watcher.Created += Watcher_Created;
			_statusTimer.Elapsed += StatusTimer_Elapsed;
		}

		private void PublishPdf()
		{
			var sourceFiles = _fileManager.GetTempFiles();
			var sourceFilesArray = sourceFiles as string[] ?? sourceFiles.ToArray();

			if (!sourceFilesArray.Any())
			{
				return;
			}

			Debug.WriteLine("Generating resulting PDF...");

			using (var content = _documentManager.GeneratePdf(sourceFilesArray))
			{
				if (content != null)
				{
					_documentPublisher.Publish(content.ToArray());
				}
			}
		}

		private void WorkProcess()
		{
			int waitResult;

			do
			{
				var sourceFiles = _fileManager.GetInputFiles();
				foreach (var fileName in sourceFiles)
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
								_status.Value = ServiceStatuses.ProcessingPdf;

								PublishPdf();

								_fileManager.ClearTemp();
								_fileManager.Delete(fileName);
							}
							else
							{
								_status.Value = ServiceStatuses.ProcessingFile;

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

				_status.Value = ServiceStatuses.Waiting;

				waitResult = WaitHandle.WaitAny(new WaitHandle[] { _stopRequested, _fileCreated }, _waitInterval);
				
				if (waitResult == WaitHandle.WaitTimeout)
				{
					try
					{
						_status.Value = ServiceStatuses.ProcessingPdf;

						PublishPdf();

						_fileManager.ClearTemp();
					}
					catch (Exception ex)
					{
						Debug.WriteLine("Exception has occured!");
						Debug.WriteLine(ex.Message);
					}
				}

			} while (waitResult != 0);
		}

		private void Watcher_Created(object sender, FileSystemEventArgs e)
		{
			_fileCreated.Set();
		}

		private void StatusTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			_statusPublisher.Publish(_status);
		}

		public void Start()
		{
			_worker.Start();
			_watcher.EnableRaisingEvents = true;
			_statusTimer.Enabled = true;
		}

		public void Stop()
		{
			_statusTimer.Enabled = false;
			_watcher.EnableRaisingEvents = false;
			_stopRequested.Set();
			_worker.Join();
		}
	}
}
