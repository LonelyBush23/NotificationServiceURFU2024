using System.Text;
using System.Text.Json;
using Common.RabbitMQ;
using NotificationQueue.Application.Features.Notification;
using NotificationQueue.Domain.Enums;

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
            var body = Encoding.UTF8.GetBytes(message);
            await _rabbitMQService.PublishMessageAsync(queue, body, cancellationToken);
            return true;
        }
        catch
        {
            return false;
        }
    }
}