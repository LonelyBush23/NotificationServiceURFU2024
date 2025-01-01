using Microsoft.AspNetCore.Http.HttpResults;
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
        private readonly IMessageQueue _messageQueue;

        public SendNotificationCommandHandler(IMessageQueue messageQueue)
        {
            _messageQueue = messageQueue;
        }

        public override async Task<Result.Result> Handle(SendNotificationCommand request, CancellationToken cancellationToken)
        {
            //if (string.IsNullOrWhiteSpace(request.Receiver))
            //{
            //    return Error(new ValidationError() { Data = { { nameof(request.Receiver), "Empty value" } } });
            //}
            return await _messageQueue.PublishAsync(request, cancellationToken);

        }
    }

}
