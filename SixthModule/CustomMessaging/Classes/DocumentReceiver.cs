using System.Collections.Generic;
using System.IO;
using System.Text;
using CustomMessaging.Interfaces;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CustomMessaging.Classes
{
	public class DocumentReceiver : IReceiver
	{
		private string _outputPath;

		public string OutputPath
		{
			get { return _outputPath; }
			set { _outputPath = value; }
		}

		private string _currentGuid = null;
		private readonly List<DocumentPartition> _partitions = new List<DocumentPartition>();

		protected void MakePdf()
		{
			List<byte> result = new List<byte>();
			foreach (var partition in _partitions)
			{
				result.AddRange(partition.Content);
			}

			var resultPath = Path.Combine(_outputPath, _currentGuid);
			using (var file = File.Open(resultPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
			{
				using (var writer = new StreamWriter(file))
				{
					writer.Write(result.ToArray());
				}
			}

			_partitions.Clear();
			_currentGuid = null;
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
					var partition = JsonConvert.DeserializeObject<DocumentPartition>(message);
					if (string.IsNullOrWhiteSpace(_currentGuid))
					{
						_currentGuid = partition.DocumentGuid;
					}
					else if (_currentGuid != partition.DocumentGuid)
					{
						MakePdf();
						_currentGuid = partition.DocumentGuid;
					}

					_partitions.Add(partition);
					if (partition.EndOfDocument)
					{
						MakePdf();
					}

				};
				channel.BasicConsume(queue: queueName,
									 noAck: true,
									 consumer: consumer);
			}
		}
	}
}
