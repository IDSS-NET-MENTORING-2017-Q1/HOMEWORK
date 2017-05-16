using System;
using CustomMessaging.DTO;
using System.IO;
using System.Configuration;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Threading;
using CustomMessaging.Listeners;
using CustomMessaging.Publishers;

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

		private static bool TryOpen(string filePath, int retryCount, int interval)
		{
			FileStream file = null;

			for (var i = 0; i < retryCount; i++)
			{
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

		private static void Watcher_Changed(object sender, FileSystemEventArgs e)
		{
			if (e.FullPath != settingsPath)
			{
				return;
			}

			if (!TryOpen(settingsPath, 3, 5000))
				return;

			var settingsText = File.ReadAllText(settingsPath);

			var settings = JsonConvert.DeserializeObject<SettingsDTO>(settingsText);
			settingsPublisher.Publish(settings);
		}

		private static void StatusListener_Received(object sender, StatusDTO e)
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
