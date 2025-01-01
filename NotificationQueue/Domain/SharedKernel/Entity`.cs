namespace NotificationQueue.Domain.SharedKernel
{
    public abstract class Entity<TKey> : Entity
    {
        public required TKey Id { get; set; }
    }
}
