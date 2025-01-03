﻿using System.Text;
using System.Text.Json;
using Common.RabbitMQ;
using NotificationQueue.Application.Features.Notification;
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

        string message;

        if (notification.Channel == NotificationChannel.Email)
        {
            message = JsonSerializer.Serialize(
                new { subject = notification.Message, body = notification.Message, recipientEmail = notification.Receiver }
            );
        }
        else if (notification.Channel == NotificationChannel.Telegram) {
            message = JsonSerializer.Serialize(
                new { chatId = notification.Receiver, text = notification.Message }
            );
        }
        else
        {
            message = JsonSerializer.Serialize(notification);

        }

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