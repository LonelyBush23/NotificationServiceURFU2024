using NotificationQueue.Application.Features.Notification;
using NotificationQueue.Application.Result;
using NotificationQueue.Domain.Repositories;

namespace NotificationQueue.Infrastructure.RabbitMQ
{
    public class MessageQueue : IMessageQueue
    {
        private readonly IRabbitMqPublisher _rabbitMqService;
        private readonly INotificationRepository _notificationRepository;

        public MessageQueue(IRabbitMqPublisher rabbitMqService, INotificationRepository notificationRepository)
        {
            _rabbitMqService = rabbitMqService;
            _notificationRepository = notificationRepository;
        }

        public async Task<Result> PublishAsync(SendNotificationCommand request, CancellationToken cancellationToken)
        {

            var result = await _rabbitMqService.SendMessageAsync(request, cancellationToken);

            return result ? Result.Success() : Result.Error(new ValidationError() { Data = { { nameof(request.Message), "Failed to send a message" } } });
        }
    }
}
