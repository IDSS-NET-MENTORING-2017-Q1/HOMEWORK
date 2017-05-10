namespace Scanner.Classes
{
	public class FileManagerFactory
	{
		private string _outputPath;
		private string _tempPath;
		private string _corruptedPath;

		public FileManagerFactory(string outputPath, string tempPath, string corruptedPath)
		{
			_outputPath = outputPath;
			_tempPath = tempPath;
			_corruptedPath = corruptedPath;
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

		public FileManager Create(string inputPath)
		{
			return new FileManager(inputPath, _outputPath, _tempPath, _corruptedPath);
		}
	}
}
