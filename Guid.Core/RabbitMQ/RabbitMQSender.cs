using Guid.Core.Interfaces;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guid.Core.RabbitMQ
{
    public class RabbitMQSender : ISender
    {
        private readonly ApplicationSettings applicationSettings;
        public RabbitMQSender(ApplicationSettings rabbitMQSettings)
        {
            this.applicationSettings = rabbitMQSettings;
        }
        public void Send(string sender, string receiver, string message)
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
                //Declares the queue
                channel.QueueDeclare(queue: receiver, // The name of the queue
                            durable: true, // true if we are declaring a durable queue(the queue will survive a server restart)
                            exclusive: false, // true if we are declaring an exclusive queue (restricted to this connection)
                            autoDelete: false, // true if we are declaring an auto delete queue (server will delete it when no longer in use)
                            arguments: null); // other properties for the queue

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true; // Marking messages as persistent doesn't fully guarantee that a message won't be lost.
                                              // Although it tells RabbitMQ to save the message to disk,
                                              // there is still a short time window when RabbitMQ has accepted a message and hasn't saved it yet.

                var body = Encoding.UTF8.GetBytes($"{message} from {sender}"); // Converts message to byte array
                //Publish message
                channel.BasicPublish(exchange: "", // the exchange to publish the message to
                            routingKey: receiver, // the routing key
                            basicProperties: properties, // other properties for the message
                            body: body); // the message body

                Console.WriteLine($"Sent: {message} Thread: {Thread.CurrentThread.ManagedThreadId} from {sender}");
            }
        }

        public void Send()
        {
            throw new NotImplementedException();
        }
    }
}
