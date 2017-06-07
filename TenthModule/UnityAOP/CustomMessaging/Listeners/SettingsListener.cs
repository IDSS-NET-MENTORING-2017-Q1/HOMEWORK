using System;
using System.Text;
using CustomMessaging.Interfaces;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using CustomMessaging.DTO;

namespace CustomMessaging.Listeners
{
	public class SettingsListener : IListener<SettingsDTO>
	{
		private IConnection _connection;
		private IModel _channel;
		private EventingBasicConsumer _consumer;

		public void Start()
		{
			var factory = new ConnectionFactory() { HostName = "localhost" };

			_connection = factory.CreateConnection();
			_channel = _connection.CreateModel();

			_channel.ExchangeDeclare(exchange: "settings_exchange", type: "fanout");

			var queueName = _channel.QueueDeclare().QueueName;
			_channel.QueueBind(queue: queueName,
								  exchange: "settings_exchange",
								  routingKey: "");

			_consumer = new EventingBasicConsumer(_channel);
			_consumer.Received += (model, ea) =>
			{
				var body = ea.Body;
				var message = Encoding.UTF8.GetString(body);
				var settings = JsonConvert.DeserializeObject<SettingsDTO>(message);
				if (Received != null)
				{
					Received(this, settings);
				}
			};

			_channel.BasicConsume(queue: queueName,
								 noAck: true,
								 consumer: _consumer);
		}
		
		public void Dispose()
		{
			_channel.Dispose();
			_connection.Dispose();
		}

		public event EventHandler<SettingsDTO> Received;
	}
}
