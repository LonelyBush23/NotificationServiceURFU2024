using System;
using System.Text.Json;
using Common.RabbitMQ;
using Common.RabbitMQ.Domain.Entities;
using Common.RabbitMQ.Domain.Enums;
using Telegram.Bot;
using TelegramSender.Domain;

namespace TelegramSender.Infrastructure.RabbitMQ;

public class RabbitMQListener : BackgroundService
{
    private readonly IRabbitMQService _rabbitMQService;
    private readonly IServiceProvider _serviceProvider;

    public RabbitMQListener(IRabbitMQService rabbitMQService, IServiceProvider serviceProvider)
    {
        _rabbitMQService = rabbitMQService;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        string queue = RBQueue.TelegramQueue.ToString();

        var queueListener = ListenToQueueWithRetriesAsync(queue, cancellationToken);

        await queueListener;
    }

    private async Task ListenToQueueWithRetriesAsync(string queue, CancellationToken cancellationToken)
    {
        await _rabbitMQService.SubscribeToQueueAsync(
            queue: queue,
            messageHandler: async (message) =>
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var telegramService = scope.ServiceProvider.GetRequiredService<ITelegramSender>();
                    var publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();


                    Console.WriteLine($"Processing message from {queue}: {message}");


                    var notification = JsonSerializer.Deserialize<Notification>(message)
                        ?? throw new ArgumentNullException(nameof(Notification), $"Failed to deserialize message {message}");

                    await telegramService.SendMessageAsync(new TelegramMessage(notification.Receiver, notification.Message));

                    notification.SentAt = DateTimeOffset.UtcNow;
                    await publisher.PublishMessageAsync(notification, Exchange.ProcessedExchange.ToString(), RBQueue.ProcessedQueue.ToString());

                    Console.WriteLine($"Message sent: {message}");
                }
            },
            cancellationToken
        );
    }
}