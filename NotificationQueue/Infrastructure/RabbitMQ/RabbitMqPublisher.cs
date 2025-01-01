using RabbitMQ.Client;
using System.Text;

namespace NotificationQueue.Infrastructure.RabbitMQ
{
    public class RabbitMqPublisher
    {
        private readonly IConnectionFactory _connectionFactory;

        public RabbitMqPublisher(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        //public void Publish(string exchange, string routingKey, string message)
        //{
        //    using var connection = _connectionFactory.CreateConnection();
        //    using var channel = connection.CreateModel();

        //    channel.ExchangeDeclare(exchange, ExchangeType.Direct);

        //    var body = Encoding.UTF8.GetBytes(message);
        //    channel.BasicPublish(exchange, routingKey, null, body);
        //}
    }
}
