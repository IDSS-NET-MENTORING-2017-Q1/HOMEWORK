namespace Scanner.Classes
{
	public class FileManagerFactory
	{
		private string _tempPath;
		private string _corruptedPath;

		public FileManagerFactory(string tempPath, string corruptedPath)
		{
			_tempPath = tempPath;
			_corruptedPath = corruptedPath;
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
			return new FileManager(inputPath, _tempPath, _corruptedPath);
		}
	}
}
