using System.Text;
using CustomMessaging.Interfaces;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CustomMessaging.Classes
{
	public class SettingsReceiver : IReceiver
	{
		private Settings _settings;

		public Settings Settings
		{
			get { return _settings; }
		}

		public void Receive()
		{
			var factory = new ConnectionFactory() { HostName = "localhost" };
			using (var connection = factory.CreateConnection())
			using (var channel = connection.CreateModel())
			{
				channel.ExchangeDeclare(exchange: "documents", type: "fanout");

				var queueName = channel.QueueDeclare().QueueName;
				channel.QueueBind(queue: queueName,
								  exchange: "documents",
								  routingKey: "");

				var consumer = new EventingBasicConsumer(channel);
				consumer.Received += (model, ea) =>
				{
					var body = ea.Body;
					var message = Encoding.UTF8.GetString(body);
					_settings = JsonConvert.DeserializeObject<Settings>(message);
				};

				channel.BasicConsume(queue: queueName,
									 noAck: true,
									 consumer: consumer);
			}
		}
	}
}
