using Common.RabbitMQ.Domain.Entities;
using Common.RabbitMQ.Domain.Enums;
using System.Text;
using System.Text.Json;

namespace Common.RabbitMQ
{
    public class Publisher : IPublisher
    {
        private readonly IRabbitMQService _rabbitMQService;

        public Publisher(IRabbitMQService rabbitMQService)
        {
            _rabbitMQService = rabbitMQService;
        }

        public async Task<bool> PublishMessageAsync<T>(T message, string exchange, string routingKey, CancellationToken cancellationToken = default)
        {
            var stringMessage = JsonSerializer.Serialize(message);

            try
            {
                var body = Encoding.UTF8.GetBytes(stringMessage);

                await _rabbitMQService.PublishMessageAsync(exchange, routingKey, routingKey, body, cancellationToken);

                return true;
            }
            catch
            {
                Console.WriteLine($"exchange '{exchange}' or queue '{routingKey}' does not exist");
                return false;
            }
        }
    }
}
