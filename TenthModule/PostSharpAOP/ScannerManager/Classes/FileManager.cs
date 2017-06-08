using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using CustomMessaging.Aspects;

namespace Scanner.Classes
{
	public class FileManager
	{
		private string _inputPath;
		private string _tempPath;
		private string _corruptedPath;

		public string InputPath
		{
			get
			{
				return _inputPath;
			}
			set
			{
				_inputPath = value;
			}
		}

		public string TempPath
		{
			get
			{
				return _tempPath;
			}
			set
			{
				_tempPath = value;
			}
		}

		public string CorruptedPath
		{
			get
			{
				return _corruptedPath;
			}
			set
			{
				_corruptedPath = value;
			}
		}

		[LogMethod]
		public bool TryOpen(string filePath, int retryCount, int interval) 
		{
			FileStream file = null;

			for (var i = 0; i < retryCount; i++) {
				try 
				{
					file = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
					file.Close();
					return true;
				}
				catch (Exception ex)
				{
					Debug.WriteLine("Exception has occured!");
					Debug.WriteLine(ex.Message);

					Thread.Sleep(interval);
				}
				finally
				{
					if (file != null)
					{
						file.Close();
					}
				}
			}

			return false;
		}

		public FileManager()
			: this(null, null, null)
		{

		}

		public FileManager(string inputPath)
			: this(inputPath, null, null)
		{

		}

		public FileManager(string inputPath, string tempPath, string corruptedPath)
		{
			var basePath = AppDomain.CurrentDomain.BaseDirectory;

			if (string.IsNullOrWhiteSpace(inputPath))
			{
				inputPath = Path.Combine(basePath, "in");
			}
			if (!Directory.Exists(inputPath))
			{
				Directory.CreateDirectory(inputPath);
			}

			if (string.IsNullOrWhiteSpace(tempPath))
			{
				tempPath = Path.Combine(basePath, "temp");
			}
			if (!Directory.Exists(tempPath))
			{
				Directory.CreateDirectory(tempPath);
			}

			if (string.IsNullOrWhiteSpace(corruptedPath))
			{
				corruptedPath = Path.Combine(basePath, "corrupted");
			}
			if (!Directory.Exists(corruptedPath))
			{
				Directory.CreateDirectory(corruptedPath);
			}

			_inputPath = inputPath;
			_tempPath = tempPath;
			_corruptedPath = corruptedPath;
		}

		[LogMethod]
		protected IEnumerable<string> GetFiles(string path)
		{
			return Directory.EnumerateFiles(path);
		}

		[LogMethod]
		public IEnumerable<string> GetInputFiles()
		{
			return GetFiles(_inputPath);
		}

		[LogMethod]
		public IEnumerable<string> GetTempFiles()
		{
			return GetFiles(_tempPath);
		}

		[LogMethod]
		public IEnumerable<string> GetCorruptedFiles()
		{
			return GetFiles(_corruptedPath);
		}

		[LogMethod]
		protected bool MoveTo(string source, string destination)
		{
			if (!File.Exists(source))
			{
				return false;
			}

			File.Move(source, destination);

			return true;
		}

		[LogMethod]
		protected string GenerateFileName(string originalName)
		{
			var extension = Path.GetExtension(originalName);
			return Guid.NewGuid().ToString() + extension;
		}

		[LogMethod]
		public bool MoveToTemp(string sourcePath)
		{
			var tempFile = Path.Combine(_tempPath, GenerateFileName(sourcePath));
			
			return MoveTo(sourcePath, tempFile);		
		}

		[LogMethod]
		public bool MoveToCorrupted(string sourcePath)
		{
			var corruptedFile = Path.Combine(_corruptedPath, GenerateFileName(sourcePath));
			return MoveTo(sourcePath, corruptedFile);		
		}

		[LogMethod]
		public bool Delete(string sourcePath)
		{
			if (!File.Exists(sourcePath))
			{
				return false;
			}

			File.Delete(sourcePath);

			return true;
		}

		[LogMethod]
		public void ClearTemp()
		{
			foreach (string fileName in GetTempFiles())
			{
				File.Delete(fileName);
			}
		}
	}
}
