using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FifthModule.Classes
{
	public class FileManager
	{
		private string _inputPath;
		private string _outputPath;
		private string _tempPath;

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

		public string OutputPath
		{
			get
			{
				return _outputPath;
			}
			set
			{
				_outputPath = value;
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

		public FileManager()
			: this(null, null, null)
		{

		}

		public FileManager(string inputPath, string outputPath)
			: this(inputPath, outputPath, null)
		{

		}

		public FileManager(string inputPath, string outputPath, string tempPath)
		{
			string basePath = AppDomain.CurrentDomain.BaseDirectory;

			if (string.IsNullOrWhiteSpace(inputPath))
			{
				inputPath = Path.Combine(basePath, "in");
			}
			if (!Directory.Exists(inputPath))
			{
				Directory.CreateDirectory(inputPath);
			}

			if (string.IsNullOrWhiteSpace(outputPath))
			{
				outputPath = Path.Combine(basePath, "out");
			}
			if (!Directory.Exists(outputPath))
			{
				Directory.CreateDirectory(outputPath);
			}

			if (string.IsNullOrWhiteSpace(tempPath))
			{
				tempPath = Path.Combine(basePath, "temp");
			}
			if (!Directory.Exists(tempPath))
			{
				Directory.CreateDirectory(tempPath);
			}

			_inputPath = inputPath;
			_outputPath = outputPath;
			_tempPath = tempPath;
		}

		protected IEnumerable<string> GetFiles(string path)
		{
			return Directory.EnumerateFiles(path);
		}

		public IEnumerable<string> GetInputFiles()
		{
			return GetFiles(_inputPath);
		}

		public IEnumerable<string> GetOutputFiles()
		{
			return GetFiles(_outputPath);
		}

		public IEnumerable<string> GetTempFiles()
		{
			return GetFiles(_tempPath);
		}

		public bool MoveToTemp(string fileName)
		{
			string inputFile = Path.Combine(_inputPath, fileName);
			string tempFile = Path.Combine(_tempPath, fileName);

			if (!File.Exists(inputFile))
			{
				return false;
			}

			File.Move(inputFile, tempFile);
			return true;
		}

		public bool ClearTemp()
		{
			foreach (string fileName in GetTempFiles())
			{
				File.Delete(fileName);
			}
		}
	}
}
