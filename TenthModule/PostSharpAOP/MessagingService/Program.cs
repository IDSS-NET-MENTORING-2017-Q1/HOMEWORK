using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Threading;
using CustomMessaging.DTO;
using CustomMessaging.Listeners;
using CustomMessaging.Publishers;
using Newtonsoft.Json;

namespace MessagingService
{
	internal class Program
	{
		private static string _settingsPath;
		private static string _statusesPath;
		private static string _outputPath;

		private static readonly DocumentListener DocumentListener = new DocumentListener();
		private static readonly StatusListener StatusListener = new StatusListener();
		private static readonly SettingsPublisher SettingsPublisher = new SettingsPublisher();
		private static readonly FileSystemWatcher Watcher = new FileSystemWatcher();

		private static void Main(string[] args)
		{
			var basePath = AppDomain.CurrentDomain.BaseDirectory;
			_settingsPath = ConfigurationManager.AppSettings["settingsFile"];
			_statusesPath = ConfigurationManager.AppSettings["statusesFile"];
			_outputPath = ConfigurationManager.AppSettings["outputPath"];

			if (string.IsNullOrWhiteSpace(_settingsPath) || !File.Exists(_settingsPath))
			{
				return;
			}
			if (string.IsNullOrWhiteSpace(_statusesPath))
			{
				_statusesPath = Path.Combine(basePath, "statuses.txt");
			}
			if (string.IsNullOrWhiteSpace(_outputPath))
			{
				_outputPath = Path.Combine(basePath, "out");
			}
			if (!Directory.Exists(_outputPath))
			{
				Directory.CreateDirectory(_outputPath);
			}

			DocumentListener.OutputPath = _outputPath;

			using (DocumentListener)
			using (StatusListener)
			{
				DocumentListener.Start();
				StatusListener.Start();

				StatusListener.Received += StatusListener_Received;

				Watcher.Path = Path.GetDirectoryName(_settingsPath);
				Watcher.Filter = "*.json";
				Watcher.Changed += Watcher_Changed;
				Watcher.EnableRaisingEvents = true;

				Console.WriteLine("Press [enter] to exit...");
				Console.ReadLine();
				
				Watcher.EnableRaisingEvents = false;
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
			if (e.FullPath != _settingsPath)
			{
				return;
			}

			if (!TryOpen(_settingsPath, 3, 5000))
				return;

			var settingsText = File.ReadAllText(_settingsPath);

			var settings = JsonConvert.DeserializeObject<SettingsDto>(settingsText);
			SettingsPublisher.Publish(settings);
		}

		private static void StatusListener_Received(object sender, StatusDto e)
		{
			using (var writer = File.AppendText(_statusesPath))
			{
				writer.WriteLine("Status Received on: {0}", DateTime.Now);
				writer.WriteLine("Service name: {0}", e.ServiceName);
				writer.WriteLine("Status: {0}", e.Value);
			}
		}
	}
}
