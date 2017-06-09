using System;
using System.Text;
using CustomMessaging.Aspects;
using CustomMessaging.DTO;
using CustomMessaging.Interfaces;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CustomMessaging.Listeners
{
	public class StatusListener : IListener<StatusDto>
	{
		private IConnection _connection;
		private IModel _channel;
		private EventingBasicConsumer _consumer;

		[LogMethod]
		public void Start()
		{
			var factory = new ConnectionFactory { HostName = "localhost" };

			_connection = factory.CreateConnection();
			_channel = _connection.CreateModel();
			_channel.ExchangeDeclare(exchange: "statuses_exchange", type: "fanout");

			var queueName = _channel.QueueDeclare().QueueName;
			_channel.QueueBind(queue: queueName,
							  exchange: "statuses_exchange",
							  routingKey: "");

			_consumer = new EventingBasicConsumer(_channel);
			_consumer.Received += (model, ea) =>
			{
				var body = ea.Body;
				var message = Encoding.UTF8.GetString(body);
				var status = JsonConvert.DeserializeObject<StatusDto>(message);

				if (Received != null)
				{
					Received(this, status);
				}
			};

			_channel.BasicConsume(queue: queueName,
								 noAck: true,
								 consumer: _consumer);
		}

		[LogMethod]
		public void Dispose()
		{
			_channel.Dispose();
			_connection.Dispose();
		}

		public event EventHandler<StatusDto> Received;
	}
}
