using NotificationQueue.Domain.Entities;
using NotificationQueue.Domain.Repositories;

namespace NotificationQueue.Infrastructure.Repositories
{
    public class NotificationRepository : EFRepository<ProcessedNotification, ServerDbContext>, INotificationRepository
    {
        private readonly ServerDbContext _context;
        public NotificationRepository(ServerDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
