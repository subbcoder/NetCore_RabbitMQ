using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = "10.10.11.18", UserName = "testuser", Password = "P@ssw0rd" };
        using (var connection = factory.CreateConnection())
        {
            Console.WriteLine("Enter command 'exit' to stop sending meesages");

            string message = string.Empty;
            do
            {
                SendNewMessage(connection, message);
                Console.Write("Enter your next message:");
                message = Console.ReadLine();
            } 
            while (!message.Equals("exit", StringComparison.OrdinalIgnoreCase));
        }
    }
    private static void SendMessage(IConnection connection, string message)
    {
        if (string.IsNullOrEmpty(message))
            return;
        using (var channel = connection.CreateModel())
        {
            channel.ExchangeDeclare("SalesOrder", ExchangeType.Fanout);

            var result = channel.QueueDeclare(queue: "hello",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null); var body =
                                    Encoding.UTF8.GetBytes(message);

            string queueName = result.QueueName;

            channel.QueueBind(queueName, "SalesOrder", "");

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            channel.BasicPublish(exchange: "",
                                 routingKey: "hello",
                                 basicProperties: properties,
                                 body: body);
            Console.WriteLine(" [x] Sent {0}", message);
        }
    }

    private static void SendNewMessage(IConnection connection, string message)
    {
        if (string.IsNullOrEmpty(message))
            return;
        using (var channel = connection.CreateModel())
        {
            channel.ExchangeDeclare("SalesOrder", ExchangeType.Fanout);

            channel.BasicPublish("SalesOrder", "", false, null, Encoding.UTF8.GetBytes(message));
        }
    }
}
