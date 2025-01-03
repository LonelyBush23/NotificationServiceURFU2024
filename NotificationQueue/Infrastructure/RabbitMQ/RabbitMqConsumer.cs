using Common.RabbitMQ;

namespace NotificationQueue.Infrastructure.RabbitMQ
{
    public class RabbitMqConsumer : BackgroundService
    {
        private readonly IRabbitMQService _rabbitMQService;

        public RabbitMqConsumer(IRabbitMQService rabbitMQService)
        {
            _rabbitMQService = rabbitMQService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var channel = await _rabbitMQService.GetChannelAsync();
            
            

        }
    }
}
