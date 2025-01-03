using System;
using System.Text.Json;
using Common.RabbitMQ;
using EmailSender.Domain;

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
        string queue = "Email";

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

                    Console.WriteLine($"Processing message from {queue}: {message}");
                    var email = JsonSerializer.Deserialize<EmailMessage>(message);

                    Console.WriteLine($"Deserialized email: {email}");

                    if (email == null)
                    {
                        throw new ArgumentNullException(nameof(EmailMessage), $"Failed to deserialize message {message}");
                    }

                    await emailService.SendAsync(email);
                }
            },
            cancellationToken
        );
    }
}
