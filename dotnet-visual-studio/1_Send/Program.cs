using System;
using RabbitMQ.Client;
using System.Text;

class Program
{
    public static void Main()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };

        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.ExchangeDeclare("tutorial.exchange", ExchangeType.Direct, true, false, null);
            channel.QueueDeclare("tutorial.queue", true, false, false, null);
            channel.QueueBind("tutorial.queue", "tutorial.exchange", "");

            while (true)
            {
                Console.WriteLine("Enter message or type 'x' to exit: ");
                string message = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(message))
                {
                    if (message.Trim().ToLower() == "x")
                        break;

                    var messages = message.Split(' ');

                    foreach (string part in messages)
                    {
                        if (string.IsNullOrWhiteSpace(part))
                            continue;

                        var address = new PublicationAddress(ExchangeType.Direct, "tutorial.exchange", "");
                        var properties = channel.CreateBasicProperties();
                        properties.Persistent = true;
                        properties.ContentType = "text/plain";

                        var body = Encoding.UTF8.GetBytes(part);
                        channel.BasicPublish(address, properties, body);
                        Console.WriteLine($" [x] Sent {part}");
                    }
                }
            }
        }
    }
}
