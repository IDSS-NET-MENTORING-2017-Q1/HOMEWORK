using System;
using System.Configuration;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;
using Topshelf;
using System.Collections.Generic;
using Newtonsoft.Json;
using Scanner.Classes;
using System.Diagnostics;
using CustomMessaging.Classes;
using System.Linq;

namespace Scanner
{
	class Program
	{
		static bool IsUnc(string sourcePath)
		{
			var uri = new Uri(sourcePath);
			return uri.IsUnc;
		}

		static void Main(string[] args)
		{
			var logPath = ConfigurationManager.AppSettings["logPath"];
			if (string.IsNullOrWhiteSpace(logPath))
			{
				logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");
			}

			var sourcesPath = ConfigurationManager.AppSettings["sourcesPath"];

			IEnumerable<Paths> paths = null;
			if (File.Exists(sourcesPath)) {
				try
				{
					paths = JsonConvert.DeserializeObject<List<Paths>>(File.ReadAllText(sourcesPath));
				}
				catch (Exception ex)
				{
					Debug.WriteLine("Exception has occured!");
					Debug.WriteLine(ex.Message);
					return;
				}
			}

			var fileManagers = paths.Where(o => !IsUnc(o.InputPath)).Select(o => new FileManager(
				o.InputPath,
				o.TempPath,
				o.CorruptedPath
			));

			var fileTarget = new FileTarget {
				Name = "Default",
				FileName = logPath,
				Layout = "${date} ${message} ${onexception:inner=${exception:format=toString}}"
			};

			var logConfig = new LoggingConfiguration();
			logConfig.AddTarget(fileTarget);
			logConfig.AddRuleForAllLevels(fileTarget);

			var logFactory = new LogFactory(logConfig);

			var documentPublisher = new DocumentPublisher();
			var statusPublisher = new StatusPublisher();
			var documentManager = new DocumentManager();
			var barcodeManager = new BarcodeManager();
			var settingsListener = new SettingsListener();

			HostFactory.Run(
				conf =>
				{
					conf.StartAutomaticallyDelayed();
					conf.SetServiceName("StreamingScannerService");
					conf.SetDisplayName("Streaming Scanner Service");
					conf.RunAsLocalSystem();
					conf.EnableServiceRecovery(recovery =>
					{
						recovery
							.RestartService(1)
							.RestartService(3)
							.RestartService(5)
							.SetResetPeriod(1);
					});

					conf.Service<ScannerManager>(callback =>
					{
						callback.ConstructUsing(() => new ScannerManager(
							fileManagers, 
							documentManager, 
							barcodeManager, 
							documentPublisher, 
							statusPublisher, 
							settingsListener));
						callback.WhenStarted(service => service.Start());
						callback.WhenStopped(service => service.Stop());
					}).UseNLog(logFactory);
				}
			);
		}
	}
}
