using NotificationQueue.Application.Infrastructure.Cqs;
using NotificationQueue.Application.Result;
using NotificationQueue.Domain.Enums;
using NotificationQueue.Infrastructure.RabbitMQ;

namespace NotificationQueue.Application.Features.Notification
{
    public class SendNotificationCommand : Command
    {
        public required string Receiver { get; set; }
        public required string Message { get; set; }
        public NotificationChannel Channel { get; set; }
    }

    public class ValidationError : Error
    {
        public override string Type => nameof(ValidationError);
    }

    public sealed class SendNotificationCommandHandler : CommandHandler<SendNotificationCommand>
    {
        private readonly IRabbitMqPublisher _publisher;

        public SendNotificationCommandHandler(IRabbitMqPublisher publisher)
        {
            _publisher = publisher;
        }

        public override async Task<Result.Result> Handle(SendNotificationCommand request, CancellationToken cancellationToken)
        {
            var notification = new Domain.Entities.Notification
            {
                Receiver = request.Receiver,
                Message = request.Message,
                Channel = request.Channel
            };

            var result = await _publisher.SendMessageAsync(notification, cancellationToken);
            return result ? Result.Result.Success() : Result.Result.Error(new ValidationError() { Data = { { nameof(request.Message), "Failed to send a message" } } });
        }
    }

}
