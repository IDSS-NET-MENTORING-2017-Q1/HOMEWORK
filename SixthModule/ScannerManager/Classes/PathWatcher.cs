﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using CustomMessaging;
using CustomMessaging.DTO;
using CustomMessaging.Interfaces;

using SystemTimer = System.Timers.Timer;
using CustomMessaging.Enums;

namespace Scanner.Classes
{
	public class PathWatcher
	{
		private StatusDTO _status;

		private int _waitInterval = 10000;
		private int _statusInterval = 10000;

		private SystemTimer _statusTimer;
		private ManualResetEvent _stopRequested = new ManualResetEvent(false);
		private AutoResetEvent _fileCreated = new AutoResetEvent(false);
		private FileSystemWatcher _watcher;
		private Thread _worker;

		private FileManager _fileManager;
		private BarcodeManager _barcodeManager;
		private DocumentManager _documentManager;
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

		public PathWatcher(string inputPath)
		{
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

			if (sourceFiles.Count() <= 0)
			{
				return;
			}

			Debug.WriteLine("Generating resulting PDF...");

			var content = _documentManager.GeneratePdf(sourceFiles).ToArray();

			_documentPublisher.Publish(content);
		}

		private void WorkProcess()
		{
			int waitResult;
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

		void StatusTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
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
