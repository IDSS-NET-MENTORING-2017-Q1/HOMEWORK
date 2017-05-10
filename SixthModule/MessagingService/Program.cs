
using System;
using System.Text;
using MessagingService.Classes;
using RabbitMQ.Client;
namespace MessagingService
{
	class Program
	{
		static void Main(string[] args)
		{
			var documentReceiver = new DocumentReceiver();
			var statusReceiver = new StatusReceiver();
			var settingsPublisher = new SettingsPublisher();

			documentReceiver.Listen();
			statusReceiver.Listen();
			settingsPublisher.Listen();

			Console.WriteLine("Press [enter] to exit.");
			Console.ReadLine();
		}
	}
}
