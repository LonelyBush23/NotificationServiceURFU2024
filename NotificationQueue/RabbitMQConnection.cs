using RabbitMQ.Client;

namespace NotificationQueue
{
    public class RabbitMQConnection
    {
        private readonly IConnection _connection;

        public RabbitMQConnection()
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };
            _connection = factory.CreateConnection();
        }

        public IModel CreateModel() => _connection.CreateModel();
    }
}
