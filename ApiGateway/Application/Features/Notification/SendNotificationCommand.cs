using ApiGateway.Application.Infrastructure.Cqs;
using ApiGateway.Application.Infrastructure.Result;
using Common.RabbitMQ;
using Common.RabbitMQ.Domain.Enums;


namespace ApiGateway.Application.Features.Notification
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
        private readonly IPublisher _publisher;

        public SendNotificationCommandHandler(IPublisher publisher)
        {
            _publisher = publisher;
        }

        public override async Task<Result> Handle(SendNotificationCommand command, CancellationToken cancellationToken)
        {
            if (!Enum.IsDefined(typeof(NotificationChannel), command.Channel))
            {
                throw new ArgumentException($"Invalid channel: {command.Channel}");
            }

            var notification = new Common.RabbitMQ.Domain.Entities.Notification
            {
                Receiver = command.Receiver,
                Message = command.Message,
                Channel = command.Channel
            };

            string routingKey = "";

            switch (notification.Channel)
            {
                case NotificationChannel.Telegram:
                    routingKey = RBQueue.TelegramQueue.ToString();
                    break;
                case NotificationChannel.Email:
                    routingKey = RBQueue.EmailQueue.ToString();
                    break;
            }

            var result = await _publisher.PublishMessageAsync(
                notification,
                Exchange.NotificationExchange.ToString(),
                routingKey,
                cancellationToken);

            return result ? Result.Success() : Result.Error(new ValidationError() { Data = { { nameof(command.Message), "Failed to send a message" } } });
        }
    }

}
