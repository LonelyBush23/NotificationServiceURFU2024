﻿using NotificationQueue.Application.Features.Notification;

namespace NotificationQueue.Infrastructure.RabbitMQ
{
    public interface IRabbitMqPublisher
    {
        Task<bool> SendMessageAsync(SendNotificationCommand command, CancellationToken cancellationToken = default);
        Task<bool> SendMessageAsync(string message, string queue, CancellationToken cancellationToken = default);
    }
}