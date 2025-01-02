﻿using NotificationQueue.Domain.Enums;
using NotificationQueue.Domain.SharedKernel;

namespace NotificationQueue.Domain.Entities
{
    public class Notification : Entity<long>, IAggregateRoot
    {
        public Notification() {}
        public string Receiver { get; set; }
        public string Message { get; set; }
        public NotificationChannel Channel { get; set; }
        public NotificationStatusType Status { get; set; }
        public int RetryCount { get; set; }
        public DateTimeOffset? SentAt { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
