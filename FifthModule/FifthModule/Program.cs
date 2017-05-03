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

			string logPath = ConfigurationManager.AppSettings["logPath"];
			if (string.IsNullOrWhiteSpace(logPath))
			{
				logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");
			}

			FileTarget logTarget = new FileTarget(logPath);

			LoggingConfiguration logConfig = new LoggingConfiguration();
			logConfig.AddTarget(logTarget);
			logConfig.AddRuleForAllLevels(logTarget);

			LogFactory logFactory = new LogFactory(logConfig);

			HostFactory.Run(
				conf =>
				{
					conf.Service<StreamingScannerService>(() => new StreamingScannerService()).UseNLog(logFactory);
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
