using System;
using System.Text;
using System.Timers;
using CustomMessaging.Classes;
using RabbitMQ.Client;

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

			settingsTimer.Elapsed += SettingsTimer_Elapsed;
			settingsTimer.Enabled = true;

			Console.WriteLine("Press [enter] to exit...");
			Console.ReadLine();
		}

		static void SettingsTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			settingsPublisher.Publish(settings);
		}
	}
}
