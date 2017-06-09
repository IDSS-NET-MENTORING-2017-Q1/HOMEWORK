using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CustomMessaging.Aspects;
using CustomMessaging.DTO;
using CustomMessaging.Interfaces;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace CustomMessaging.Publishers
{
	public class DocumentPublisher : IPublisher<IEnumerable<byte>>
	{
		[LogMethod]
		public void Publish(IEnumerable<byte> value)
		{
			var documentGuid = Guid.NewGuid().ToString();

			var factory = new ConnectionFactory { HostName = "localhost" };
			using (var connection = factory.CreateConnection())
			using (var channel = connection.CreateModel())
			{
				channel.ExchangeDeclare(exchange: "documents_exchange", type: "fanout");

				var buffer = value;
				while (buffer.Count() > 128)
				{
					var partition = new DocumentPartitionDto
					{
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
					var partition = new DocumentPartitionDto
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
