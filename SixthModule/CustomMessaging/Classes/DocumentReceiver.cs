using System.Collections.Generic;
using System.Text;
using CustomMessaging.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CustomMessaging.Classes
{
	public class DocumentReceiver : IReceiver<IEnumerable<byte>>
	{
		public IEnumerable<byte> Receive()
		{
			var factory = new ConnectionFactory() { HostName = "localhost" };
			using (var connection = factory.CreateConnection())
			using (var channel = connection.CreateModel())
			{
				channel.QueueDeclare(queue: "hello",
									 durable: false,
									 exclusive: false,
									 autoDelete: false,
									 arguments: null);

				var consumer = new EventingBasicConsumer(channel);
				consumer.Received += (model, ea) =>
				{
					var body = ea.Body;
					var message = Encoding.UTF8.GetString(body);
				};
				channel.BasicConsume(queue: "hello",
									 noAck: true,
									 consumer: consumer);
			}

			throw new System.NotImplementedException();
		}
	}
}
