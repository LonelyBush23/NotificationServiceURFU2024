using Common.RabbitMQ.Domain.Enums;
using NotificationQueue.Domain.Enums;
using NotificationQueue.Domain.SharedKernel;

namespace NotificationQueue.Domain.Entities
{
    public class ProcessedNotification : Entity<long>, IAggregateRoot
    {
        public string Receiver { get; set; }
        public string Message { get; set; }
        public NotificationChannel Channel { get; set; }
        public NotificationStatusType Status { get; set; } = NotificationStatusType.Processing;
        public int RetryCount { get; set; } = 0;
        public DateTimeOffset? SentAt { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
