using Microsoft.Extensions.Hosting;
using NotificationQueue.Domain.Enums;
using NotificationQueue.Infrastructure.RabbitMQ;
using NotificationQueue.Infrastructure.RabbitMQ.Base;

public class RabbitMqBackgroundService : BackgroundService
{
    private readonly IRabbitMQService _rabbitMQService;

    public RabbitMqBackgroundService(IRabbitMQService rabbitMQService)
    {
        _rabbitMQService = rabbitMQService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        List<string> queues = Enum.GetNames(typeof(NotificationChannel)).ToList();

        var listeningTasks = queues.Select(queue =>
            ListenToQueueAsync(queue, stoppingToken)
        );

        await Task.WhenAll(listeningTasks);
    }

    private async Task ListenToQueueAsync(string queue, CancellationToken cancellationToken)
    {
        await _rabbitMQService.SubscribeToQueue(
            queue: queue,
            messageHandler: (message) =>
            {
                Console.WriteLine($"Processing message from {queue}: {message}");
                return Task.CompletedTask;
            },
            cancellationToken
        );
    }
}
