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
using CustomMessaging.DTO;
using System.Linq;
using CustomMessaging.Publishers;
using CustomMessaging.Listeners;
using Microsoft.Practices.Unity;
using CustomMessaging.Interfaces;
using Microsoft.Practices.Unity.InterceptionExtension;
using CustomMessaging.Unity;
using Scanner.Interfaces;

namespace Scanner
{
	class Program
	{
		static bool IsUnc(string sourcePath)
		{
			var uri = new Uri(sourcePath);
			return uri.IsUnc;
		}

		static IUnityContainer ConfigureContainer()
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
			container.RegisterType<IScannerManager, ScannerManager>(new Interceptor<InterfaceInterceptor>(),
				new InterceptionBehavior<LoggingInterceptionBehavior>());
			container.RegisterType<IBarcodeManager, BarcodeManager>(new Interceptor<InterfaceInterceptor>(),
				new InterceptionBehavior<LoggingInterceptionBehavior>());
			container.RegisterType<IFileManager, FileManager>(new Interceptor<InterfaceInterceptor>(),
				new InterceptionBehavior<LoggingInterceptionBehavior>());
			container.RegisterType<IListener<SettingsDTO>, SettingsListener>(new Interceptor<InterfaceInterceptor>(),
				new InterceptionBehavior<LoggingInterceptionBehavior>());
			return container;
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
