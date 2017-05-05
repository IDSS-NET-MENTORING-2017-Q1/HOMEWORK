using System;
using System.Configuration;
using System.IO;
using FifthModule.Classes;
using NLog;
using NLog.Config;
using NLog.Targets;
using Topshelf;

namespace FifthModule
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

			var inputPath = ConfigurationManager.AppSettings["inputPath"];
			var outputPath = ConfigurationManager.AppSettings["outputPath"];
			var tempPath = ConfigurationManager.AppSettings["tempPath"];
			var corruptedPath = ConfigurationManager.AppSettings["corruptedPath"];

			var fileTarget = new FileTarget {
				Name = "Default",
				FileName = logPath,
				Layout = "${date} ${message} ${onexception:inner=${exception:format=toString}}"
			};

			var logConfig = new LoggingConfiguration();
			logConfig.AddTarget(fileTarget);
			logConfig.AddRuleForAllLevels(fileTarget);

			var logFactory = new LogFactory(logConfig);

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
						callback.ConstructUsing(() => new ScannerManager(inputPath, outputPath, tempPath, corruptedPath));
						callback.WhenStarted(service => service.Start());
						callback.WhenStopped(service => service.Stop());
					}).UseNLog(logFactory);
				}
			);
		}
	}
}
