using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace FifthModule.Classes
{
	public class StreamingScannerService : ServiceControl
	{
		private FileManager _fileManager;

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

		public StreamingScannerService()
		{
			
		}

		public StreamingScannerService(FileManager fileManager)
		{

		}

		public StreamingScannerService(string inputPath, string outputPath, string tempPath)
		{
			_fileManager = new FileManager(inputPath, outputPath, tempPath);
		}

		public bool Start(HostControl hostControl)
		{
			throw new NotImplementedException();
		}

		public bool Stop(HostControl hostControl)
		{
			throw new NotImplementedException();
		}
	}
}
