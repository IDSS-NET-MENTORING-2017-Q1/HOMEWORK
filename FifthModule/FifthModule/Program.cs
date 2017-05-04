using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

			var logTarget = new FileTarget(logPath);

			var logConfig = new LoggingConfiguration();
			logConfig.AddTarget(logTarget);
			logConfig.AddRuleForAllLevels(logTarget);

			var logFactory = new LogFactory(logConfig);

			HostFactory.Run(
				conf =>
				{
					conf.Service<ScannerManager>(
						() => new ScannerManager(inputPath, outputPath, tempPath, corruptedPath))
						.UseNLog(logFactory);
					conf.StartAutomaticallyDelayed();
					conf.SetDisplayName("Streaming Scanner Service");
					conf.RunAsLocalSystem();
					conf.EnableServiceRecovery(recovery =>
					{
						recovery.SetResetPeriod(0);
						recovery.RestartService(1);
					});
				}
			);
		}
	}
}
