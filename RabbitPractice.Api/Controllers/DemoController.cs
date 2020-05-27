using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitPractice.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DemoController : ControllerBase
    {
        [HttpGet("{name}")]
        public IActionResult Send(string name)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "admin",
                Password = "123456",
                VirtualHost="rt"
            };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "hello1",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                string message = $"Hello,{name}!";
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: "hello1",
                                     basicProperties: null,
                                     body: body);
                return Ok($"[x] Sent {message}");
            }
        }

        /// <summary>
        /// 延迟队列
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("{name}")]
        public IActionResult Delayed(string name)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "admin",
                Password = "123456",
                VirtualHost = "dm"
            };

            using var connection = factory.CreateConnection();
            try
            {
                using var channel = connection.CreateModel();
                channel.CallbackException += (sender, e) =>
                {
                    Console.WriteLine("CallbackException:", e);
                };
                //var queueArgs = new Dictionary<string, object> {
                //    { "x-dead-letter-exchange", WORK_EXCHANGE },
                //    { "x-message-ttl", RETRY_DELAY }
                //};
                var queueArgs = new Dictionary<string, object>();
                long ms = 50000; //最大延迟毫秒值 4294967295
                queueArgs.Add("x-expires", ms);
                queueArgs.Add("x-message-ttl", 1200);//队列上消息过期时间，应小于队列过期时间(ms)  
                queueArgs.Add("x-dead-letter-exchange", "exchange-direct");//过期消息转向路由  
                queueArgs.Add("x-dead-letter-routing-key", "routing-delay");//过期消息转向路由相匹配routingkey 
                channel.QueueDeclare(queue: "hello3",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: queueArgs);
                var message = $"Hello, {name} at {DateTime.Now}";
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                    routingKey: "hello3",
                    basicProperties: null,
                    body: body);

                return Ok($"[x] Sent {message}");

            }
            catch (Exception e)
            {
                //Console.WriteLine($"Send exception:{e}");
                return Ok($"Exception:{e}");
            }
        }
    }
}
