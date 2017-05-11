using System;
using System.Timers;
using CustomMessaging.Classes;

namespace MessagingService
{
	class Program
	{
		static DocumentListener documentListener = new DocumentListener();
		static StatusListener statusListener = new StatusListener();
		static SettingsPublisher settingsPublisher = new SettingsPublisher();
		static Settings settings = new Settings()
		{
			EndOfDocument = "EndOfDocument",
			Timeout = 10000
		};
		static Timer settingsTimer = new Timer(10000)
		{
			Enabled = false,
			AutoReset = true
		};

		static void Main(string[] args)
		{
			documentListener.Start();
			statusListener.Start();

			statusListener.Received += StatusListener_Received;

			settingsTimer.Elapsed += SettingsTimer_Elapsed;
			settingsTimer.Enabled = true;

			Console.WriteLine("Press [enter] to exit...");
			Console.ReadLine();

			settingsTimer.Enabled = false;
			documentListener.Stop();
			statusListener.Stop();
		}

		private static void StatusListener_Received(object sender, Status e)
		{
			Console.WriteLine("Status Received!");
			Console.WriteLine(string.Format("Service name: {0}", e.ServiceName));
			Console.WriteLine(string.Format("Status: {0}", e.Value));
		}

		static void SettingsTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			settingsPublisher.Publish(settings);
		}
	}
}
