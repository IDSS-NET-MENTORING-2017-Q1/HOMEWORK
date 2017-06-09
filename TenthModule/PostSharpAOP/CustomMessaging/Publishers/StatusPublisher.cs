using System.Text;
using CustomMessaging.Aspects;
using CustomMessaging.DTO;
using CustomMessaging.Interfaces;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace CustomMessaging.Publishers
{
	public class StatusPublisher : IPublisher<StatusDto>
	{
		[LogMethod]
		public void Publish(StatusDto value)
		{
			var factory = new ConnectionFactory { HostName = "localhost" };
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
