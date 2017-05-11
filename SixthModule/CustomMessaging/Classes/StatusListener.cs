using System.Collections.Generic;
using System.Text;
using System.Linq;
using CustomMessaging.Interfaces;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Threading;

namespace CustomMessaging.Classes
{
	public class StatusListener : IListener<Status>
	{
		private ICollection<Status> _statuses;

		private ManualResetEvent _stopRequested;
		private Thread _worker;
		private object _lockObj;

		public StatusListener()
		{
			_worker = new Thread(WorkProcess);
			_stopRequested = new ManualResetEvent(false);
			_statuses = new List<Status>();
			_lockObj = new object();
		}

		public ICollection<Status> Statuses
		{
			get
			{
				return _statuses;
			}
			set
			{
				_statuses = value;
			}
		}

		private void WorkProcess()
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

					if (Received != null)
					{
						Received(this, status);
					}

					lock (_lockObj)
					{
						var registeredStatus = _statuses.FirstOrDefault(o => o.ServiceName == status.ServiceName);
						if (registeredStatus != null)
						{
							registeredStatus.Value = status.Value;
						}
						else
						{
							_statuses.Add(status);
						}
					}
				};
				channel.BasicConsume(queue: queueName,
									 noAck: true,
									 consumer: consumer);
			}

			_stopRequested.WaitOne();
		}

		public void Start()
		{
			_worker.Start();
		}


		public void Stop()
		{
			_stopRequested.Set();
		}

		public event System.EventHandler<Status> Received;
	}
}
