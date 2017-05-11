using System.Collections.Generic;
using System.Text;
using System.Linq;
using CustomMessaging.Interfaces;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CustomMessaging.Classes
{
	public class StatusReceiver : IReceiver
	{
		private ICollection<Status> _statuses = new List<Status>();

		public ICollection<Status> Statuses
		{
			get { return _statuses; }
			set { _statuses = value; }
		}

		public void Receive()
		{
			var factory = new ConnectionFactory() { HostName = "localhost" };
			using (var connection = factory.CreateConnection())
			using (var channel = connection.CreateModel())
			{
				channel.ExchangeDeclare(exchange: "statuses", type: "fanout");

				var queueName = channel.QueueDeclare().QueueName;
				channel.QueueBind(queue: queueName,
								  exchange: "statuses",
								  routingKey: "");

				var consumer = new EventingBasicConsumer(channel);
				consumer.Received += (model, ea) =>
				{
					var body = ea.Body;
					var message = Encoding.UTF8.GetString(body);
					var status = JsonConvert.DeserializeObject<Status>(message);

					var registeredStatus = _statuses.FirstOrDefault(o => o.ServiceName == status.ServiceName);
					if (registeredStatus != null)
					{
						registeredStatus.Value = status.Value;
					}
					else
					{
						_statuses.Add(status);
					}
				};
				channel.BasicConsume(queue: queueName,
									 noAck: true,
									 consumer: consumer);
			}
		}
	}
}
