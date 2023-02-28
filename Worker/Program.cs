using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

class Program
{
    private const string hostName = "10.10.11.18", userName = "testuser", password = "P@ssw0rd";
    public static void Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = hostName, UserName = userName, Password = password };
        using (var connection = factory.CreateConnection()) // Creates the RabbitMQ connection
        using (var channel = connection.CreateModel()) // Creates a channel, which is where most of the API for getting things done resides.
        {
            // We will also have to declare the queue here,
            // because this application might start first so we will make sure that the queue exists before receiving messages.
            channel.QueueDeclare(queue: "task_queue", // The name of the queue
                        durable: false, // true if we are declaring a durable queue(the queue will survive a server restart)
                        exclusive: false, // true if we are declaring an exclusive queue (restricted to this connection)
                        autoDelete: false, // true if we are declaring an auto delete queue (server will delete it when no longer in use)
                        arguments: null); // other properties (construction arguments) for the queue

            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            var consumer = new EventingBasicConsumer(channel);
            // Callback
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.Write("Received: {0}", message);

                Thread.Sleep(5000); // Our fake task that will finish every 5 seconds.
                Console.WriteLine(" [Done]"); // When task is done, displays [done].
                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            // Start receiving messages
            channel.BasicConsume(queue: "task_queue", // the name of the queue
                        autoAck: true,  // true if the server should consider messages acknowledged once delivered;
                                        //false if the server should expect explicit acknowledgements
                        consumer: consumer); // an interface to the consumer object

            Console.WriteLine("Waiting for message/s.");
            Console.ReadLine();
        }
    }
}