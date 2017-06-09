using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CustomMessaging.DTO;
using CustomMessaging.Interfaces;
using CustomMessaging.Listeners;
using CustomMessaging.Publishers;
using CustomMessaging.Unity;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using Newtonsoft.Json;
using NLog;
using NLog.Config;
using NLog.Targets;
using ScannerManager.Classes;
using ScannerManager.Interfaces;
using Topshelf;

namespace ScannerManager
{
	internal class Program
	{
		private static bool IsUnc(string sourcePath)
		{
			var uri = new Uri(sourcePath);
			return uri.IsUnc;
		}

		private static IUnityContainer ConfigureContainer()
		{
			var container = new UnityContainer();
			container.AddNewExtension<Interception>();
			container.RegisterType<IPublisher<IEnumerable<byte>>, DocumentPublisher>(new Interceptor<InterfaceInterceptor>(),
				new InterceptionBehavior<LoggingInterceptionBehavior>());
			container.RegisterType<IPublisher<StatusDTO>, StatusPublisher>(new Interceptor<InterfaceInterceptor>(),
				new InterceptionBehavior<LoggingInterceptionBehavior>());
			container.RegisterType<IDocumentManager, DocumentManager>(new Interceptor<InterfaceInterceptor>(),
				new InterceptionBehavior<LoggingInterceptionBehavior>());
			container.RegisterType<IPathWatcher, PathWatcher>(new Interceptor<InterfaceInterceptor>(),
				new InterceptionBehavior<LoggingInterceptionBehavior>());
			container.RegisterType<IScannerManager, Classes.ScannerManager>(new Interceptor<InterfaceInterceptor>(),
				new InterceptionBehavior<LoggingInterceptionBehavior>());
			container.RegisterType<IBarcodeManager, BarcodeManager>(new Interceptor<InterfaceInterceptor>(),
				new InterceptionBehavior<LoggingInterceptionBehavior>());
			container.RegisterType<IFileManager, FileManager>(new Interceptor<InterfaceInterceptor>(),
				new InterceptionBehavior<LoggingInterceptionBehavior>());
			container.RegisterType<IListener<SettingsDTO>, SettingsListener>(new Interceptor<InterfaceInterceptor>(),
				new InterceptionBehavior<LoggingInterceptionBehavior>());
			return container;
		}

		private static void Main(string[] args)
		{
			var logPath = ConfigurationManager.AppSettings["logPath"];
			if (string.IsNullOrWhiteSpace(logPath))
			{
				logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");
			}

			var sourcesPath = ConfigurationManager.AppSettings["sourcesPath"];

			IEnumerable<Paths> paths = null;
			if (File.Exists(sourcesPath))
			{
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

			var fileTarget = new FileTarget
			{
				Name = "Default",
				FileName = logPath,
				Layout = "${date} ${message} ${onexception:inner=${exception:format=toString}}"
			};

			var logConfig = new LoggingConfiguration();
			logConfig.AddTarget(fileTarget);
			logConfig.AddRuleForAllLevels(fileTarget);

			var logFactory = new LogFactory(logConfig);
			var container = ConfigureContainer();

			if (paths == null) return;

			var fileManagers = paths.Where(o => !IsUnc(o.InputPath)).Select(o => container.Resolve<IFileManager>(
				new ParameterOverride("inputPath", o.InputPath),
				new ParameterOverride("tempPath", o.TempPath),
				new ParameterOverride("corruptedPath", o.CorruptedPath)
			));

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

					conf.Service<IScannerManager>(callback =>
					{
						callback.ConstructUsing(() => container.Resolve<IScannerManager>(
							new ParameterOverride("fileManagers", fileManagers)
						));
						callback.WhenStarted(service => service.Start());
						callback.WhenStopped(service => service.Stop());
					}).UseNLog(logFactory);
				}
			);
		}
	}
}
