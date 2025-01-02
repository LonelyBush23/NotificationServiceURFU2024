using System.Text.Json;
using Common.RabbitMQ;
using NotificationQueue.Application.Features.Notification;
using NotificationQueue.Domain.Entities;
using NotificationQueue.Domain.Enums;
using NotificationQueue.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;

public class RabbitMqBackgroundService : BackgroundService
{
    private readonly IRabbitMQService _rabbitMQService;
    private readonly IServiceProvider _serviceProvider;

    public RabbitMqBackgroundService(IRabbitMQService rabbitMQService, IServiceProvider serviceProvider)
    {
        _rabbitMQService = rabbitMQService;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        List<string> queues = Enum.GetNames(typeof(NotificationChannel)).ToList();

        var listeningTasks = queues.Select(queue =>
            ListenToQueueWithRetriesAsync(queue, stoppingToken)
        );

        await Task.WhenAll(listeningTasks);
    }

    private async Task ListenToQueueWithRetriesAsync(string queue, CancellationToken cancellationToken)
    {
        await _rabbitMQService.SubscribeToQueueAsync(
            queue: queue,
            messageHandler: async (message) =>
            {
                Console.WriteLine($"Processing message from {queue}: {message}");

                // Создание области для Scoped-сервисов
                using (var scope = _serviceProvider.CreateScope())
                {
                    var notificationRepository = scope.ServiceProvider.GetRequiredService<INotificationRepository>();

                    var command = JsonSerializer.Deserialize<SendNotificationCommand>(message);
                    var notification = new Notification
                    {
                        Receiver = command.Receiver,
                        Message = command.Message,
                        Channel = command.Channel,
                        Status = NotificationStatusType.Success,
                        RetryCount = 1,
                        CreatedAt = DateTimeOffset.UtcNow
                    };

                    Console.WriteLine($"Adding to Database: {message}");

                    await notificationRepository.AddAsync(notification, cancellationToken);
                    await notificationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
                }
            },
            cancellationToken
        );
    }
}
