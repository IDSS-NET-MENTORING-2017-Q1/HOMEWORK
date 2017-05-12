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

namespace Scanner
{
	class Program
	{
		static void Main(string[] args)
		{
			var logPath = ConfigurationManager.AppSettings["logPath"];
			if (string.IsNullOrWhiteSpace(logPath))
			{
				logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");
			}

			var outputPath = ConfigurationManager.AppSettings["outputPath"];
			var tempPath = ConfigurationManager.AppSettings["tempPath"];
			var corruptedPath = ConfigurationManager.AppSettings["corruptedPath"];
			var sourcesPath = ConfigurationManager.AppSettings["sourcesPath"];

			IEnumerable<string> sources = null;
			if (File.Exists(sourcesPath)) {
				try
				{
					sources = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(sourcesPath));
				}
				catch (Exception ex)
				{
					Debug.WriteLine("Exception has occured!");
					Debug.WriteLine(ex.Message);
				}
			}

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
			var fileManagerFactory = new FileManagerFactory(tempPath, corruptedPath);

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
							sources, 
							fileManagerFactory, 
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
