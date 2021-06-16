using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Receive
{
  class Program
  {
    static void Main(string[] args)
    {
      var factory = new ConnectionFactory() { HostName = "localhost" };
      using (var connection = factory.CreateConnection())
      using (var channel = connection.CreateModel())
      {
        channel.QueueDeclare(queue: "messageQueue",
                            durable: false,
                            exclusive: false,
                            autoDelete: false,
                            arguments: null);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
          var body = ea.Body.ToArray();
          var receivedMessage = Encoding.UTF8.GetString(body);

          if (String.IsNullOrWhiteSpace(receivedMessage))
          {
            Console.WriteLine("Empty message received!!");
          }
          else if (receivedMessage.Length < 19)
          {
            Console.WriteLine("Incorrect message received!!");
          }
          else
          {
            var name = receivedMessage.Substring(18);
            Console.WriteLine($"Hello {name}, I am your father!");
          }
        };
        channel.BasicConsume(queue: "messageQueue",
                            autoAck: true,
                            consumer: consumer);

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
      }
    }
  }
}
