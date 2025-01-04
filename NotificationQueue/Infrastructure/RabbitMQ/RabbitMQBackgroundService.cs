using System.Text.Json;
using Common.RabbitMQ;
using NotificationQueue.Domain.Entities;
using NotificationQueue.Domain.Enums;
using NotificationQueue.Domain.Repositories;
using Common.RabbitMQ.Domain.Enums;
using Common.RabbitMQ.Domain.Entities;

public class RabbitMqBackgroundService : BackgroundService
{
    private readonly IRabbitMQService _rabbitMQService;
    private readonly IServiceProvider _serviceProvider;

    public RabbitMqBackgroundService(IRabbitMQService rabbitMQService, IServiceProvider serviceProvider)
    {
        _rabbitMQService = rabbitMQService;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        string queue = RBQueue.ProcessedQueue.ToString();

        var queueListener = ListenToQueueWithRetriesAsync(queue, cancellationToken);

        await queueListener;
    }
    private async Task ListenToQueueWithRetriesAsync(string queue, CancellationToken cancellationToken)
    {
        await _rabbitMQService.SubscribeToQueueAsync(
            queue: queue,
            messageHandler: async (message) =>
            {
                Console.WriteLine($"Processing message from {queue}: {message}");

                using (var scope = _serviceProvider.CreateScope())
                {
                    var notificationRepository = scope.ServiceProvider.GetRequiredService<INotificationRepository>();

                    var notification = JsonSerializer.Deserialize<Notification>(message)
                        ?? throw new ArgumentNullException(nameof(Notification), $"Failed to deserialize message {message}");

                    var processedNotification = new ProcessedNotification{
                        Receiver = notification.Receiver,
                        Message = notification.Message,
                        Channel = notification.Channel,
                        Status = NotificationStatusType.Success,
                        RetryCount = 0,
                        SentAt = notification.SentAt,
                        CreatedAt = notification.CreatedAt
                    };

                    await notificationRepository.AddAsync(processedNotification, cancellationToken);
                    await notificationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
                }
            },
            cancellationToken
        );
    }
}
