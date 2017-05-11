using System;
using System.Text;
using System.Threading;
using CustomMessaging.Interfaces;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CustomMessaging.Classes
{
	public class SettingsListener : IListener<Settings>
	{
		private Settings _settings;

		public Settings Settings
		{
			get { return _settings; }
		}

		private ManualResetEvent _stopRequested;
		private Thread _worker;
		private object _lockObj;

		public SettingsListener()
		{
			_worker = new Thread(WorkProcess);
			_stopRequested = new ManualResetEvent(false);
			_lockObj = new object();
		}

		private void WorkProcess()
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
					lock (_lockObj)
					{
						_settings = JsonConvert.DeserializeObject<Settings>(message);
						if (Received != null)
						{
							Received(this, _settings);
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

		public event EventHandler<Settings> Received;
	}
}
