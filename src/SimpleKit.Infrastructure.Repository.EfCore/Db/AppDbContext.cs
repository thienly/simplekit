using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SimpleKit.Domain.Entities;
using SimpleKit.Domain.Events;

namespace SimpleKit.Infrastructure.Repository.EfCore.Db
{
    public abstract class AppDbContext : DbContext
    {
        private ILogger<AppDbContext> _logger;

        public AppDbContext()
        {
        }

        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public abstract IDomainEventDispatcher DomainEventDispatcher { get; set; }
        public abstract void PreProcessSaveChanges();

        private void PublishUncommitedDomainEvents()
        {
            var domainEvents = base.ChangeTracker.Entries().Where(x => x.Entity is IAggregateRoot)
                .SelectMany(x => ((IAggregateRoot) x.Entity).GetUncommittedEvents()).ToList();

            foreach (var @event in domainEvents)
            {
                if (@event is IDomainEvent)
                {
                    DomainEventDispatcher.Dispatch(@event);
                }
            }
        }

        public override int SaveChanges()
        {
            PublishUncommitedDomainEvents();
            PreProcessSaveChanges();
            var affectedRows = base.SaveChanges();
            ProcessAfterSaveChanges();
            return affectedRows;
        }

        public virtual void ProcessAfterSaveChanges()
        {
            foreach (var entityEntry in this.ChangeTracker.Entries())
            {
                entityEntry.State = EntityState.Detached;
            }
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            PublishUncommitedDomainEvents();
            var affectedRows = await base.SaveChangesAsync(cancellationToken);
            ProcessAfterSaveChanges();
            return affectedRows;
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var affectedRows = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            ProcessAfterSaveChanges();
            return affectedRows;
        }
    }
}