using System;
using RabbitMQ.Client;
using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Send
{
  public class Program
  {
    private readonly ILogger<Program> logger;
    private readonly MessageBuilder serviceA;
    private readonly QueueingService serviceB;

    static void Main(string[] args)
    {
      var host = CreateHostBuilder(args).Build();
      host.Services.GetRequiredService<Program>().Run();
    }

    public Program(ILogger<Program> logger, MessageBuilder serviceA, QueueingService serviceB)
    {
      this.logger = logger;
      this.serviceA = serviceA;
      this.serviceB = serviceB;
    }

    public void Run()
    {
      string name = serviceA.ReadName();
      string message = serviceA.ConstructMessage(name);
      serviceB.SendMessageToQueue(message);
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
      return Host.CreateDefaultBuilder(args)
          .ConfigureServices(services =>
          {
            services.AddTransient<Program>();
            services.AddTransient<MessageBuilder>();
            services.AddTransient<QueueingService>();
          });
    }
  }

  public class MessageBuilder
  {
    public string ReadName()
    {
      Console.WriteLine("Please enter your name on the next line and press [Enter].");
      string name = Console.ReadLine();
      return name;
    }

    public string ConstructMessage(string name)
    {
      if (string.IsNullOrWhiteSpace(name)) return string.Empty;

      string message = $"Hello my name is, {name}";
      return message;
    }
  }

  public class QueueingService
  {
    public void SendMessageToQueue(string message)
    {
      if (string.IsNullOrWhiteSpace(message)) return;

      var factory = new ConnectionFactory() { HostName = "localhost" };
      using (var connection = factory.CreateConnection())
      {
        using (var channel = connection.CreateModel())
        {
          channel.QueueDeclare(queue: "messageQueue",
           durable: false,
           exclusive: false,
           autoDelete: false,
           arguments: null);


          var body = Encoding.UTF8.GetBytes(message);

          channel.BasicPublish(exchange: "",
                              routingKey: "messageQueue",
                              basicProperties: null,
                              body: body);
          Console.WriteLine(" [x] Sent {0}", message);
        }
      }

      Console.WriteLine(" Press [enter] to exit.");
      Console.ReadLine();
    }
  }
}
