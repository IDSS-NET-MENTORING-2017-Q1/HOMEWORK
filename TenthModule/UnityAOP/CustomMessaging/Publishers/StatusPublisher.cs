using System.Text;
using CustomMessaging.Interfaces;
using Newtonsoft.Json;
using RabbitMQ.Client;
using CustomMessaging.DTO;
using CustomMessaging.Unity;

namespace CustomMessaging.Publishers
{
	[LogFileName("status_publisher_logs")]
	public class StatusPublisher : IPublisher<StatusDTO>
	{
		public void Publish(StatusDTO value)
		{
			var factory = new ConnectionFactory() { HostName = "localhost" };
			using (var connection = factory.CreateConnection())
			using (var channel = connection.CreateModel())
			{
				channel.ExchangeDeclare(exchange: "statuses_exchange", type: "fanout");

				var message = JsonConvert.SerializeObject(value);
				var body = Encoding.UTF8.GetBytes(message);

				channel.BasicPublish(exchange: "statuses_exchange",
										routingKey: "",
										basicProperties: null,
										body: body);
				
			}
		}
	}
}
