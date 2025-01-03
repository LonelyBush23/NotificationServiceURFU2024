using System;
using System.Text.Json;
using Common.RabbitMQ;
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
        string queue = "Telegram";

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

                    Console.WriteLine($"Processing message from {queue}: {message}");
                    var msg = JsonSerializer.Deserialize<TelegramMessage>(message);

                    Console.WriteLine($"Deserialized telegram message: {msg}");

                    if (msg == null)
                    {
                        throw new ArgumentNullException(nameof(TelegramMessage), $"Failed to deserialize message {message}");
                    }

                    await telegramService.SendMessageAsync(msg);
                }
            },
            cancellationToken
        );
    }
}