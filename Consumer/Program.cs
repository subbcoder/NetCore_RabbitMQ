using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

class Program
{
    public static void Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = "10.10.11.18", UserName = "testuser", Password = "P@ssw0rd" }; // If your broker resides on a different machine, you can specify the name or the IP address.
        using (var connection = factory.CreateConnection()) // Creates the RabbitMQ connection
        using (var channel = connection.CreateModel()) // Creates a channel, which is where most of the API for getting things done resides.
        {
            // We will also have to declare the queue here,
            // because this application might start first so we will make sure that the queue exists before receiving messages.
            channel.QueueDeclare(queue: "hello", // The name of the queue
                                 durable: false, // true if we are declaring a durable queue(the queue will survive a server restart)
                                 exclusive: false, // true if we are declaring an exclusive queue (restricted to this connection)
                                 autoDelete: false, // true if we are declaring an auto delete queue (server will delete it when no longer in use)
                                 arguments: null); // other properties (construction arguments) for the queue

            var consumer = new EventingBasicConsumer(channel);
            // Callback
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine("Received: {0}", message);
            };

            // Start receiving messages
            channel.BasicConsume(queue: "hello", // the name of the queue
                                 autoAck: true, // true if the server should consider messages acknowledged once delivered;
                                                //false if the server should expect explicit acknowledgements
                                 consumer: consumer); // an interface to the consumer object
            Console.WriteLine("Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}

 