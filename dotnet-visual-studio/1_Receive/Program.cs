using RabbitMQ.Client;
using System;
using _1_Receive;

class Program
{
    public static void Main()
    {
        var factory = new ConnectionFactory { HostName = "localhost", VirtualHost = "tutorial" };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            //channel.QueueDeclare(queue: "hello", durable: false, exclusive: false, autoDelete: false, arguments: null);
            channel.BasicQos(0, 1, false);
            Console.WriteLine(" [*] Waiting for messages.");

            //var consumer = new EventingBasicConsumer(channel);
            //consumer.Received += (model, ea) =>
            //{
            //    var body = ea.Body;
            //    var message = Encoding.UTF8.GetString(body);
            //    Console.WriteLine(" [x] Received {0}", message);
            //};
            //channel.BasicConsume(queue: "hello", noAck: true, consumer: consumer);

            var basicConsumer = new OneWayMessageReceiver(channel);
            channel.BasicConsume("tutorial.queue", false, basicConsumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
