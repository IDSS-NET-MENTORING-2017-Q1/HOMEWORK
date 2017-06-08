using System;
using System.Configuration;
using System.IO;

namespace CustomMessaging.Aspects
{
	public class Logger
	{
		private static int indentLevel;

		public static void Indent()
		{
			indentLevel++;
		}

		public static void Unindent()
		{
			indentLevel--;
		}

		public static void WriteLine(string message)
		{
			var logPath = ConfigurationManager.AppSettings["logPath"];
			if (string.IsNullOrWhiteSpace(logPath))
			{
				logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");
			}
			using (var logFile = File.AppendText(logPath))
			{
				logFile.Write(new string(' ', 3 * indentLevel));
				logFile.WriteLine(message);
			}
		}
	}
}
