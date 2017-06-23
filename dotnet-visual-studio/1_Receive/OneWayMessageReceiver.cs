using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;

namespace _1_Receive
{
    public class OneWayMessageReceiver : DefaultBasicConsumer
    {
        private readonly IModel _channel;
        private readonly Random _random = new Random();

        public OneWayMessageReceiver(IModel channel)
        {
            _channel = channel;
        }

        public override void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey,
            IBasicProperties properties, byte[] body)
        {
            Console.WriteLine($"Message received from the exchange {exchange}");
            Console.WriteLine($"Consumer tag: {consumerTag}");
            Console.WriteLine($"Delivery tag: {deliveryTag}");
            Console.WriteLine($"Message: {Encoding.UTF8.GetString(body)}");

            _channel.BasicAck(deliveryTag, false);

            Thread.Sleep(_random.Next(2000,5000));
        }
    }
}
