using System;
using System.Text;
using CustomMessaging.Interfaces;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CustomMessaging.Classes
{
	public class SettingsListener : IListener<Settings>
	{
		private IConnection _connection;
		private IModel _channel;
		private EventingBasicConsumer _consumer;

		public void Start()
		{
			var factory = new ConnectionFactory() { HostName = "localhost" };

			_connection = factory.CreateConnection();
			_channel = _connection.CreateModel();

			_channel.ExchangeDeclare(exchange: "documents", type: "fanout");

			var queueName = _channel.QueueDeclare().QueueName;
			_channel.QueueBind(queue: queueName,
								  exchange: "documents",
								  routingKey: "");

			_consumer = new EventingBasicConsumer(_channel);
			_consumer.Received += (model, ea) =>
			{
				var body = ea.Body;
				var message = Encoding.UTF8.GetString(body);
				var settings = JsonConvert.DeserializeObject<Settings>(message);
				if (Received != null)
				{
					Received(this, settings);
				}
			};

			_channel.BasicConsume(queue: queueName,
								 noAck: true,
								 consumer: _consumer);
		}

		public void Stop()
		{
			_channel.Dispose();
			_connection.Dispose();
		}

		public event EventHandler<Settings> Received;
	}
}
