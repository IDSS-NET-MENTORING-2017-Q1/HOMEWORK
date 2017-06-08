using System.Text;
using CustomMessaging.Interfaces;
using RabbitMQ.Client;
using Newtonsoft.Json;
using CustomMessaging.DTO;
using CustomMessaging.Aspects;

namespace CustomMessaging.Publishers
{
	public class SettingsPublisher : IPublisher<SettingsDTO>
	{
		[LogMethod]
		public void Publish(SettingsDTO value)
		{
			var factory = new ConnectionFactory() { HostName = "localhost" };
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
