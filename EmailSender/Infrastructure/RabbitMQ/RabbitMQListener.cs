using System.Text.Json;
using Common.RabbitMQ;
using Common.RabbitMQ.Domain.Entities;
using Common.RabbitMQ.Domain.Enums;
using EmailSender.Domain;
using Microsoft.AspNetCore.Mvc;

namespace EmailSender.Infrastructure.RabbitMQ;

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
        string queue = RBQueue.EmailQueue.ToString();

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
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailSender>();
                    var publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();

                    Console.WriteLine($"Processing message from {queue}: {message}");

                    var notification = JsonSerializer.Deserialize<Notification>(message)
                        ?? throw new ArgumentNullException(nameof(Notification), $"Failed to deserialize message {message}");

                    await emailService.SendAsync(new EmailMessage(notification.Message, notification.Message, notification.Receiver));

                    notification.SentAt = DateTimeOffset.UtcNow;

                    await publisher.PublishMessageAsync(notification, Exchange.ProcessedExchange.ToString(), RBQueue.ProcessedQueue.ToString());

                    Console.WriteLine($"Message sent: {message}");
                }
            },
            cancellationToken
        );
    }
}
