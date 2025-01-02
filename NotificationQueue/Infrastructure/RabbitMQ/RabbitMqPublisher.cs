using System.Text;
using System.Text.Json;
using NotificationQueue.Application.Features.Notification;
using NotificationQueue.Domain.Enums;
using NotificationQueue.Infrastructure.RabbitMQ.Base;
using RabbitMQ.Client;

namespace NotificationQueue.Infrastructure.RabbitMQ;

public class RabbitMqPublisher : IRabbitMqPublisher
{
    private readonly IRabbitMQService _rabbitMQService;

    public RabbitMqPublisher(IRabbitMQService rabbitMQService)
    {
        _rabbitMQService = rabbitMQService;
    }

    public async Task<bool> SendMessageAsync(SendNotificationCommand command, CancellationToken cancellationToken = default)
    {
        if (!Enum.IsDefined(typeof(NotificationChannel), command.Channel))
        {
            throw new ArgumentException($"Invalid channel: {command.Channel}");
        }

        string queue = command.Channel.ToString();

        var message = JsonSerializer.Serialize(command);
        return await SendMessageAsync(message, queue, cancellationToken);
    }

    public async Task<bool> SendMessageAsync(string message, string queue, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var channel = await _rabbitMQService.GetChannel(cancellationToken);
            await _rabbitMQService.DeclareQueue(channel, queue, cancellationToken);

            var body = Encoding.UTF8.GetBytes(message);
            await channel.BasicPublishAsync("", queue, false, body, cancellationToken);
            return true;
        }
        catch
        {
            return false;
        }
    }
}