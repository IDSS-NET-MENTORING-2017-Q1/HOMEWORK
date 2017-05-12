using System;
using CustomMessaging.Classes;
using System.IO;
using System.Configuration;
using Newtonsoft.Json;

namespace MessagingService
{
	class Program
	{
		static string settingsPath;
		static string statusesPath;
		static string outputPath;

		static DocumentListener documentListener = new DocumentListener();
		static StatusListener statusListener = new StatusListener();
		static SettingsPublisher settingsPublisher = new SettingsPublisher();
		static FileSystemWatcher watcher = new FileSystemWatcher();

		static void Main(string[] args)
		{
			var basePath = AppDomain.CurrentDomain.BaseDirectory;
			settingsPath = ConfigurationManager.AppSettings["settingsFile"];
			statusesPath = ConfigurationManager.AppSettings["statusesFile"];
			outputPath = ConfigurationManager.AppSettings["outputPath"];

			if (string.IsNullOrWhiteSpace(settingsPath) || !File.Exists(settingsPath))
			{
				return;
			}
			if (string.IsNullOrWhiteSpace(statusesPath))
			{
				statusesPath = Path.Combine(basePath, "statuses.txt");
			}
			if (string.IsNullOrWhiteSpace(outputPath))
			{
				outputPath = Path.Combine(basePath, "out");
			}
			if (!Directory.Exists(outputPath))
			{
				Directory.CreateDirectory(outputPath);
			}

			documentListener.OutputPath = outputPath;

			using (documentListener)
			using (statusListener)
			{
				documentListener.Start();
				statusListener.Start();

				statusListener.Received += StatusListener_Received;

				watcher.Path = Path.GetDirectoryName(settingsPath);
				watcher.Filter = "*.json";
				watcher.Changed += Watcher_Changed;
				watcher.EnableRaisingEvents = true;

				Console.WriteLine("Press [enter] to exit...");
				Console.ReadLine();
				
				watcher.EnableRaisingEvents = false;
			}
		}

		private static void Watcher_Changed(object sender, FileSystemEventArgs e)
		{
			if (e.FullPath != settingsPath)
			{
				return;
			}

			var settingsText = File.ReadAllText(settingsPath);
			var settings = JsonConvert.DeserializeObject<Settings>(settingsText);
			settingsPublisher.Publish(settings);
		}

		private static void StatusListener_Received(object sender, Status e)
		{
			using (var writer = File.AppendText(statusesPath))
			{
				writer.WriteLine(string.Format("Status Received on: {0}", DateTime.Now));
				writer.WriteLine(string.Format("Service name: {0}", e.ServiceName));
				writer.WriteLine(string.Format("Status: {0}", e.Value));
			}
		}
	}
}
