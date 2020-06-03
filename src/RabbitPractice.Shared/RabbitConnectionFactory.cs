using RabbitMQ.Client;

namespace RabbitPractice.Shared
{
    public class RabbitConnectionFactory
    {
        public static ConnectionFactory GetConnectionFactory
        {
            get
            {
                var factory = new ConnectionFactory()
                {
                    HostName = "localhost",
                    UserName = "guest",
                    Password = "guest",
                    VirtualHost = "/"
                };
                return factory;
            }
        }
    }
}
