using System;
using RabbitMQ.Client;
using System.Text;

class Program
{
    public static void Main()
    {
        var factory = new ConnectionFactory {HostName = "localhost", VirtualHost = "tutorial"};

        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            SetUpFanOutExchange(channel);
            //SetUpDirectExchange(channel);   
        }
    }

    private static void SetUpFanOutExchange(IModel channel)
    {
        channel.ExchangeDeclare("tutorial.fanout.exchange", ExchangeType.Fanout, true, false, null);
        channel.QueueDeclare("tutorial.queue.acc", true, false, false, null);
        channel.QueueDeclare("tutorial.queue.mgmt", true, false, false, null);
        channel.QueueBind("tutorial.queue.acc", "tutorial.fanout.exchange", "");
        channel.QueueBind("tutorial.queue.mgmt", "tutorial.fanout.exchange", "");

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

                    var address = new PublicationAddress(ExchangeType.Fanout, "tutorial.fanout.exchange", "");
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

    private static void SetUpDirectExchange(IModel channel)
    {
        channel.ExchangeDeclare("tutorial.direct.exchange", ExchangeType.Direct, true, false, null);
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

                    var address = new PublicationAddress(ExchangeType.Direct, "tutorial.direct.exchange", "");
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
