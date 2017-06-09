using System.Text;
using CustomMessaging.Aspects;
using CustomMessaging.DTO;
using CustomMessaging.Interfaces;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace CustomMessaging.Publishers
{
	public class SettingsPublisher : IPublisher<SettingsDto>
	{
		[LogMethod]
		public void Publish(SettingsDto value)
		{
			var factory = new ConnectionFactory { HostName = "localhost" };
			using (var connection = factory.CreateConnection())
			using (var channel = connection.CreateModel())
			{
				channel.ExchangeDeclare(exchange: "settings_exchange", type: "fanout");

				var message = JsonConvert.SerializeObject(value);
				var body = Encoding.UTF8.GetBytes(message);

				channel.BasicPublish(exchange: "settings_exchange",
									 routingKey: "",
									 basicProperties: null,
									 body: body);
			}
		}
	}
}
