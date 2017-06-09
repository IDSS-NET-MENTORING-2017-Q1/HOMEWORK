using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using CustomMessaging.Aspects;
using CustomMessaging.DTO;
using CustomMessaging.Interfaces;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CustomMessaging.Listeners
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
		
		private readonly List<DocumentPartitionDto> _partitions;

		public DocumentListener() : this(null) { }

		public DocumentListener(string outputPath)
		{
			_partitions = new List<DocumentPartitionDto>();

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

		[LogMethod]
		protected void MakePdf(string guid)
		{
			var result = new List<byte>();
			foreach (var partition in _partitions.Where(o => o.DocumentGuid == guid))
			{
				result.AddRange(partition.Content);
			}

			var path = Path.Combine(_outputPath, guid + ".pdf");
			var content = result.ToArray();

			File.WriteAllBytes(path, content);

			_partitions.RemoveAll(o => o.DocumentGuid == guid);
		}

		[LogMethod]
		public void Start()
		{
			var factory = new ConnectionFactory { HostName = "localhost" };

			_connection = factory.CreateConnection();
			_channel = _connection.CreateModel();

			_channel.ExchangeDeclare(exchange: "ps_documents_exchange", type: "fanout");

			var queueName = _channel.QueueDeclare().QueueName;
			_channel.QueueBind(queue: queueName,
							  exchange: "ps_documents_exchange",
							  routingKey: "");

			_consumer = new EventingBasicConsumer(_channel);
			_consumer.Received += (model, ea) =>
			{
				var body = ea.Body;
				var message = Encoding.UTF8.GetString(body);
				DocumentPartitionDto partition;

				try
				{
					partition = JsonConvert.DeserializeObject<DocumentPartitionDto>(message);
				}
				catch (Exception ex)
				{
					Debug.WriteLine("Exception has occured!");
					Debug.WriteLine(ex.Message);
					return;
				}


				_partitions.Add(partition);
				if (partition.EndOfDocument)
				{
					MakePdf(partition.DocumentGuid);
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
	}
}
