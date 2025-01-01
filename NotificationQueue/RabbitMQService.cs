using RabbitMQ.Client;
using System.Text;

namespace NotificationQueue
{
    public class RabbitMQService
    {
        private readonly RabbitMQConnection _connection;

        public RabbitMQService(RabbitMQConnection connection)
        {
            _connection = connection;
        }

        public async Task PublishAsync(string queueName, string message)
        {
            using var channel = _connection.CreateModel();
            channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false);

            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);

            await Task.CompletedTask;
        }

    }
}
