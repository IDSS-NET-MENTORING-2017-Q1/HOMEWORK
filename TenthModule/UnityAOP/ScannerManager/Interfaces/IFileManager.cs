using System.Collections.Generic;

namespace Scanner.Interfaces
{
	public interface IFileManager
	{
		string CorruptedPath { get; set; }
		string InputPath { get; set; }
		string TempPath { get; set; }

		void ClearTemp();
		bool Delete(string sourcePath);
		IEnumerable<string> GetCorruptedFiles();
		IEnumerable<string> GetInputFiles();
		IEnumerable<string> GetTempFiles();
		bool MoveToCorrupted(string sourcePath);
		bool MoveToTemp(string sourcePath);
		bool TryOpen(string filePath, int retryCount, int interval);
	}
}