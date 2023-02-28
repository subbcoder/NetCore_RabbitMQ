using RabbitMQ.Client;
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
            Console.WriteLine("Please enter your message. Type 'exit' to exit.");
            //Declares the queue
            channel.QueueDeclare(queue: "task_queue", // The name of the queue
                        durable: false, // true if we are declaring a durable queue(the queue will survive a server restart)
                        exclusive: false, // true if we are declaring an exclusive queue (restricted to this connection)
                        autoDelete: false, // true if we are declaring an auto delete queue (server will delete it when no longer in use)
                        arguments: null); // other properties for the queue

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            while (true)
            {
                string message = Console.ReadLine();
                if (message?.ToUpper() == "EXIT")
                {
                    break;
                }

                var body = Encoding.UTF8.GetBytes(message); // Converts message to byte array

                //Publish message
                channel.BasicPublish(exchange: "", // the exchange to publish the message to
                              routingKey: "task_queue", // the routing key
                              basicProperties: properties, // other properties for the message
                              body: body); // the message body
            }
        }
    }
}