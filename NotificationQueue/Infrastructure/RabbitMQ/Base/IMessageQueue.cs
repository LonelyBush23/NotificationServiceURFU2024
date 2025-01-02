using NotificationQueue.Application.Features.Notification;
using NotificationQueue.Application.Result;

namespace NotificationQueue.Infrastructure.RabbitMQ
{
    public interface IMessageQueue
    {
        Task<Result> PublishAsync(SendNotificationCommand request, CancellationToken cancellationToken);
    }
}
