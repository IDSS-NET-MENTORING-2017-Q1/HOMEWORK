using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CustomMessaging.Interfaces;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CustomMessaging.Classes
{
	public class DocumentListener : IListener
	{
		private string _outputPath;

		private IConnection _connection;
		private IModel _channel;
		private EventingBasicConsumer _consumer;

		public string OutputPath
		{
			get { return _outputPath; }
			set { _outputPath = value; }
		}

		private string _currentGuid = null;
		private readonly List<DocumentPartition> _partitions;

		public DocumentListener() : this(null) { }

		public DocumentListener(string outputPath)
		{
			_partitions = new List<DocumentPartition>();

			if (string.IsNullOrWhiteSpace(outputPath))
			{
				var basePath = AppDomain.CurrentDomain.BaseDirectory;

				if (string.IsNullOrWhiteSpace(outputPath))
				{
					outputPath = Path.Combine(basePath, "out");
				}
				if (!Directory.Exists(outputPath))
				{
					Directory.CreateDirectory(outputPath);
				}
			}
			else
			{
				_outputPath = outputPath;
			}
		}

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
			_channel.BasicConsume(queue: queueName,
								 noAck: true,
								 consumer: _consumer);
		}

		public void Dispose()
		{
			_channel.Dispose();
			_connection.Dispose();
		}
	}
}
