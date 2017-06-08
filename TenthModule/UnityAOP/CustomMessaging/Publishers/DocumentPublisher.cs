using System.Collections.Generic;
using CustomMessaging.Interfaces;
using System.Text;
using RabbitMQ.Client;
using Newtonsoft.Json;
using System.Linq;
using System;
using CustomMessaging.DTO;
using CustomMessaging.Unity;

namespace CustomMessaging.Publishers
{
	[LogFileName("document_publisher_logs")]
	public class DocumentPublisher : IPublisher<IEnumerable<byte>>, IIdentifiable
	{
		private Guid _objectGuid = Guid.NewGuid();

		public string ObjectGuid
		{
			get
			{
				return _objectGuid.ToString();
			}
		}

		public void Publish(IEnumerable<byte> value)
		{
			var documentGuid = Guid.NewGuid().ToString();

			var factory = new ConnectionFactory() { HostName = "localhost" };
			using (var connection = factory.CreateConnection())
			using (var channel = connection.CreateModel())
			{
				channel.ExchangeDeclare(exchange: "documents_exchange", type: "fanout");

				var buffer = value;
				while (buffer.Count() > 128)
				{
					var partition = new DocumentPartitionDTO() {
						Content = buffer.Take(128),
						DocumentGuid = documentGuid,
						EndOfDocument = false
					};

					var message = JsonConvert.SerializeObject(partition);
					var body = Encoding.UTF8.GetBytes(message);

					channel.BasicPublish(exchange: "documents_exchange",
										 routingKey: "",
										 basicProperties: null,
										 body: body);

					buffer = buffer.Skip(128);
				}

				if (buffer.Count() > 0)
				{
					var partition = new DocumentPartitionDTO()
					{
						Content = buffer,
						DocumentGuid = documentGuid,
						EndOfDocument = true
					};

					var message = JsonConvert.SerializeObject(partition);
					var body = Encoding.UTF8.GetBytes(message);

					channel.BasicPublish(exchange: "documents_exchange",
										 routingKey: "",
										 basicProperties: null,
										 body: body);
				}
			}
		}
	}
}
