using NotificationQueue.Domain.Entities;
using NotificationQueue.Domain.SharedKernel.Storage;

namespace NotificationQueue.Domain.Repositories
{
    public interface INotificationRepository : IRepository<Notification>
    {
    }
}
