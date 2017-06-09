using System.Collections.Generic;
using CustomMessaging.Interfaces;
using System.Text;
using RabbitMQ.Client;
using Newtonsoft.Json;
using System.Linq;
using System;
using CustomMessaging.DTO;

namespace CustomMessaging.Publishers
{
	public class DocumentPublisher : IPublisher<IEnumerable<byte>>
	{
		public void Publish(IEnumerable<byte> value)
		{
			var documentGuid = Guid.NewGuid().ToString();

			var factory = new ConnectionFactory() { HostName = "localhost" };
			using (var connection = factory.CreateConnection())
			using (var channel = connection.CreateModel())
			{
				channel.ExchangeDeclare(exchange: "documents_exchange", type: "fanout");

				var buffer = value;
				DocumentPartitionDTO partition = new DocumentPartitionDTO()
				{
					DocumentGuid = documentGuid,
					EndOfDocument = false
				};

				while (buffer.Count() > 128)
				{
					partition.Content = buffer.Take(128);

					var message = JsonConvert.SerializeObject(partition);
					var body = Encoding.UTF8.GetBytes(message);

					channel.BasicPublish(exchange: "documents_exchange",
										 routingKey: "",
										 basicProperties: null,
										 body: body);

					buffer = buffer.Skip(128).ToArray();
				}

				if (!buffer.Any()) return;
				{
					partition.Content = buffer;
					partition.EndOfDocument = true;

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
