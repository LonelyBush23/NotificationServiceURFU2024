using NotificationQueue.Application.Features.Notification;
using NotificationQueue.Application.Result;

namespace NotificationQueue.Infrastructure.RabbitMQ
{
    public class MessageQueue : IMessageQueue
    {
        public Task<Result> PublishAsync(SendNotificationCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
