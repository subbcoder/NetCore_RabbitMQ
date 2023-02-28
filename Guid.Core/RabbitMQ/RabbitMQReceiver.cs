using Guid.Core.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guid.Core.RabbitMQ
{
    public class RabbitMQReceiver : IReceiver
    {
        private readonly ApplicationSettings applicationSettings;
        public RabbitMQReceiver(ApplicationSettings applicationSettings)
        {
            this.applicationSettings = applicationSettings;
        }

        public void Receive(string name)
        {
            var factory = new ConnectionFactory()
            {
                HostName = applicationSettings.RabbitMQServer,
                UserName = applicationSettings.RabbitMQUser,
                Password = applicationSettings.RabbitMQPassword
            };
            using (var connection = factory.CreateConnection()) // Creates the RabbitMQ connection
            using (var channel = connection.CreateModel()) // Creates a channel, which is where most of the API for getting things done resides.
            {
                // We will also have to declare the queue here,
                // because this application might start first so we will make sure that the queue exists before receiving messages.
                channel.QueueDeclare(queue: name, // The name of the queue
                            durable: true, // true if we are declaring a durable queue(the queue will survive a server restart)
                            exclusive: false, // true if we are declaring an exclusive queue (restricted to this connection)
                            autoDelete: false, // true if we are declaring an auto delete queue (server will delete it when no longer in use)
                            arguments: null); // other properties (construction arguments) for the queue

                // Request a specific Quality of Service
                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                var consumer = new EventingBasicConsumer(channel);
                // Callback
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine($"Received: {message}");

                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };

                // Start receiving messages
                channel.BasicConsume(queue: name, // the name of the queue
                            autoAck: false, // true if the server should consider messages acknowledged once delivered;
                                            // false if the server should expect explicit acknowledgements
                            consumer: consumer); // an interface to the consumer object
                Console.WriteLine("Waiting for message/s.");
                Console.ReadLine();
            }
        }

        public void Receive()
        {
            throw new NotImplementedException();
        }
    }
}
