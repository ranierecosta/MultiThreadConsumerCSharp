using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleConsumer
{
	class Program
	{
		static void Main(string[] args)
		{
			var factory = new ConnectionFactory() { HostName = "localhost" };
			using (var connection = factory.CreateConnection())
			using (var channel = connection.CreateModel())
			{
				List<string> queues = new List<string>();
				queues.Add("Order");
				queues.Add("Product");

				queues.ForEach(queue =>
				{
					channel.QueueDeclare(queue: queue,
										 durable: false,
										 exclusive: false,
										 autoDelete: false,
										 arguments: null);

					BuildAndRunWorker(channel, queue, queue);
				});

				Console.ReadLine();
			}
		}

		public static void BuildAndRunWorker(IModel channel, string queue, string workerName)
		{
			var consumer = new EventingBasicConsumer(channel);
			consumer.Received += (model, ea) =>
			{
				var body = ea.Body.ToArray();
				var message = Encoding.UTF8.GetString(body);
				Console.WriteLine($" [x] Received {message}");
			};
			channel.BasicConsume(queue: queue,
								 autoAck: true,
								 consumer: consumer);
		}
	}
}