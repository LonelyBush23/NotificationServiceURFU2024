using NotificationQueue.Application.Features.Notification;
using NotificationQueue.Domain.Entities;

namespace NotificationQueue.Infrastructure.RabbitMQ
{
    public interface IRabbitMqPublisher
    {
        Task<bool> SendMessageAsync(Notification notification, CancellationToken cancellationToken = default);
        Task<bool> SendMessageAsync(string message, string routingKey, CancellationToken cancellationToken = default);
    }
}
