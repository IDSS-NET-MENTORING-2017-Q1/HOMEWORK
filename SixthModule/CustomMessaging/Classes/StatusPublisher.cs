using System.Text;
using CustomMessaging.Interfaces;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace CustomMessaging.Classes
{
	public class StatusPublisher : IPublisher<Status>
	{
		public void Publish(Status value)
		{
			var factory = new ConnectionFactory() { HostName = "localhost" };
			using (var connection = factory.CreateConnection())
			using (var channel = connection.CreateModel())
			{
				channel.ExchangeDeclare(exchange: "statuses", type: "fanout");

				var message = JsonConvert.SerializeObject(value);
				var body = Encoding.UTF8.GetBytes(message);

				channel.BasicPublish(exchange: "statuses",
										routingKey: "",
										basicProperties: null,
										body: body);
				
			}
		}
	}
}
