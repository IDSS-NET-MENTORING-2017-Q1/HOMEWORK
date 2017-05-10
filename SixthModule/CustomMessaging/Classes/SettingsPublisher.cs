using System.Text;
using CustomMessaging.Interfaces;
using RabbitMQ.Client;
using Newtonsoft.Json;

namespace CustomMessaging.Classes
{
	public class SettingsPublisher : IPublisher<Settings>
	{
		public void Publish(Settings value)
		{
			var factory = new ConnectionFactory() { HostName = "localhost" };
			using (var connection = factory.CreateConnection())
			using (var channel = connection.CreateModel())
			{
				channel.ExchangeDeclare(exchange: "logs", type: "fanout");

				var message = JsonConvert.SerializeObject(value);
				var body = Encoding.UTF8.GetBytes(message);
				channel.BasicPublish(exchange: "logs",
									 routingKey: "",
									 basicProperties: null,
									 body: body);
			}
		}
	}
}
