using System.Text;
using System.Text.Json;
using Common.RabbitMQ;
using NotificationQueue.Domain.Entities;
using NotificationQueue.Domain.Enums;

namespace NotificationQueue.Infrastructure.RabbitMQ;

public class RabbitMqPublisher : IRabbitMqPublisher
{
    private readonly IRabbitMQService _rabbitMQService;

    public RabbitMqPublisher(IRabbitMQService rabbitMQService)
    {
        _rabbitMQService = rabbitMQService;
    }

    public async Task<bool> SendMessageAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        if (!Enum.IsDefined(typeof(NotificationChannel), notification.Channel))
        {
            throw new ArgumentException($"Invalid channel: {notification.Channel}");
        }

        var newNotification = new Common.RabbitMQ.Domain.Entities.Notification
        {
            Receiver = notification.Receiver,
            Channel = (Common.RabbitMQ.Domain.Enums.NotificationChannel)notification.Channel,
            Message = notification.Message,
        };

        var message = JsonSerializer.Serialize(newNotification);
        string routingKey = notification.Channel.ToString();
        return await SendMessageAsync(message, routingKey, cancellationToken);
    }

    public async Task<bool> SendMessageAsync(string message, string routingKey, CancellationToken cancellationToken = default)
    {
        try
        {
            var body = Encoding.UTF8.GetBytes(message);

            await _rabbitMQService.PublishMessageAsync(RabbitMQConstants.NotificationExchange, routingKey, routingKey, body, cancellationToken);

            return true;
        }
        catch
        {
            return false;
        }
    }
}