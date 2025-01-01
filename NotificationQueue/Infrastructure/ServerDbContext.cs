using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using NotificationQueue.Domain.Entities;
using NotificationQueue.Domain.SharedKernel;
using NotificationQueue.Domain.SharedKernel.Storage;

namespace NotificationQueue.Infrastructure
{
    public class ServerDbContext : DbContext, IUnitOfWork
    {
        public DbSet<Notification> Notification { get; set; }

        public ServerDbContext(DbContextOptions<ServerDbContext> contextOptions) : base(contextOptions)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ServerDbContext).Assembly);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var mediator = this.GetService<IMediator>();
            await this.DispatchDomainEventsAsync(mediator);
            return await base.SaveChangesAsync(cancellationToken);
        }
    }

    public static class ServerDbContextExtensions
    {
        public static async Task DispatchDomainEventsAsync(this ServerDbContext dbContext, IMediator mediator)
        {
            var domainEntities = dbContext.ChangeTracker.Entries<Entity>().Where(x => x.Entity.DomainEvents?.Any() ?? false).ToList();
            var domainEvents = domainEntities.SelectMany(x => x.Entity.DomainEvents).ToList();
            domainEntities.ForEach(entity => entity.Entity.ClearDomainEvents());

            var tasks = domainEvents.Select(async domainEvent => await mediator.Publish(domainEvent, CancellationToken.None));
            await Task.WhenAll(tasks);
        }
    }
}
