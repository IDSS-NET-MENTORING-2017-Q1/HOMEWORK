using System.Text;
using CustomMessaging.Interfaces;
using RabbitMQ.Client;

namespace CustomMessaging.Classes
{
	public class StatusReceiver : IReceiver<ServiceStatuses>
	{
		public ServiceStatuses Receive()
		{
			var factory = new ConnectionFactory() { HostName = "localhost" };
			using (var connection = factory.CreateConnection())
			using (var channel = connection.CreateModel())
			{
				channel.ExchangeDeclare(exchange: "logs", type: "fanout");

				var queueName = channel.QueueDeclare().QueueName;
				channel.QueueBind(queue: queueName,
								  exchange: "logs",
								  routingKey: "");

				var consumer = new EventingBasicConsumer(channel);
				consumer.Received += (model, ea) =>
				{
					var body = ea.Body;
					var message = Encoding.UTF8.GetString(body);
				};
				channel.BasicConsume(queue: queueName,
									 noAck: true,
									 consumer: consumer);
			}

			return null;
		}
	}
}
