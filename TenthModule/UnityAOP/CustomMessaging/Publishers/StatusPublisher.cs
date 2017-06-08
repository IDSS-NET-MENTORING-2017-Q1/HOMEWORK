using System.Text;
using CustomMessaging.Interfaces;
using Newtonsoft.Json;
using RabbitMQ.Client;
using CustomMessaging.DTO;
using CustomMessaging.Unity;
using System;

namespace CustomMessaging.Publishers
{
	[LogFileName("status_publisher_logs")]
	public class StatusPublisher : IPublisher<StatusDTO>, IIdentifiable
	{
		private Guid _objectGuid = Guid.NewGuid();

		public string ObjectGuid
		{
			get
			{
				return _objectGuid.ToString();
			}
		}

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
