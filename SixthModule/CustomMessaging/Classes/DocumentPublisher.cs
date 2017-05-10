using System.Collections.Generic;
using CustomMessaging.Interfaces;
using System.Text;
using RabbitMQ.Client;
using Newtonsoft.Json;
using System.Linq;
using System;

namespace CustomMessaging.Classes
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
				channel.ExchangeDeclare(exchange: "logs", type: "fanout");

				var buffer = value;
				while (buffer.Count() > 128)
				{
					var partition = new DocumentPartition() {
						Content = buffer.Take(128),
						DocumentGuid = documentGuid,
						EndOfDocument = false
					};

					var message = JsonConvert.SerializeObject(value);
					var body = Encoding.UTF8.GetBytes(message);
					channel.BasicPublish(exchange: "logs",
										 routingKey: "",
										 basicProperties: null,
										 body: body);

					buffer = buffer.Skip(128);
				}

				if (buffer.Count() > 0)
				{
					var partition = new DocumentPartition()
					{
						Content = buffer,
						DocumentGuid = documentGuid,
						EndOfDocument = true
					};

					var message = JsonConvert.SerializeObject(value);
					var body = Encoding.UTF8.GetBytes(message);
					channel.BasicPublish(exchange: "logs",
										 routingKey: "",
										 basicProperties: null,
										 body: body);
				}
			}
		}
	}
}
