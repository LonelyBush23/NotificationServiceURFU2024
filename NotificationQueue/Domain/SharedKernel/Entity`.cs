﻿namespace NotificationQueue.Domain.SharedKernel
{
    public abstract class Entity<TKey> : Entity
    {
        public TKey Id { get; set; }
    }
}
