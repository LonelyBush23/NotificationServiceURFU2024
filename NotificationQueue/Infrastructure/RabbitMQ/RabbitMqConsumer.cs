using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace NotificationQueue.Infrastructure.RabbitMQ
{
    public class RabbitMqConsumer
    {
        private readonly IConnectionFactory _connectionFactory;

        public RabbitMqConsumer(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public void Consume(string queue, Func<string, Task> onMessageReceived)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                await onMessageReceived(message);
            };

            channel.BasicConsume(queue, autoAck: true, consumer);
        }
    }
}
