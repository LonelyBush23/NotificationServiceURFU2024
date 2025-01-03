using Common.RabbitMQ.Domain.Enums;

namespace Common.RabbitMQ.Domain.Entities;

public class Notification
{
    public required string Receiver { get; set; }
    public required string Message { get; set; }
    public NotificationChannel Channel { get; set; }
    public int RetryCount { get; set; } = 0;
    public DateTimeOffset? SentAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
