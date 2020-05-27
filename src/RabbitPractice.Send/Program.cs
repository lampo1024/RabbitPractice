using System;
using System.Text;
using RabbitMQ.Client;

namespace RabbitPractice.Send
{
    class Program
    {
        public static void Main()
        {
            //DeleteQueue();
            Send();
            Console.ReadLine();
        }

        public static void Send()
        {
            var input = Console.ReadLine();
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "admin",
                Password = "123456"
            };

            using (var connection = factory.CreateConnection())
            {

                try
                {
                    using var channel = connection.CreateModel();
                    channel.CallbackException += (sender, e) =>
                    {
                        Console.WriteLine("CallbackException:", e);
                    };

                    channel.QueueDeclare(queue: "hello1",
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);
                    while (!string.Equals(input, "x", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var message = $"Hello, {input} at {DateTime.Now}";
                        var body = Encoding.UTF8.GetBytes(message);

                        channel.BasicPublish(exchange: "",
                            routingKey: "hello1",
                            basicProperties: null,
                            body: body);
                        Console.WriteLine(" [x] Sent {0}", message);
                        input = Console.ReadLine();
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine($"Send exception:{e}");
                }
            }

            Console.WriteLine(" Press [enter] to exit.");
        }


        public static void DeleteQueue()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "admin",
                Password = "123456"
            };

            using (var connection = factory.CreateConnection())
            {

                try
                {
                    using var channel = connection.CreateModel();
                    channel.CallbackException += (sender, e) =>
                    {
                        Console.WriteLine("CallbackException:", e);
                    };

                    channel.QueueDelete("hello1");
                    
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Send exception:{e}");
                }
            }

            Console.WriteLine(" Press [enter] to exit.");
        }
    }
}
