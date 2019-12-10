using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
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

        public abstract void PreProcessSaveChanges();
        private void PublishUncommitedDomainEvents()
        {
            foreach (var entityEntry in base.ChangeTracker.Entries())
            {
                if (entityEntry.Entity is IAggregateRoot)
                {
                    var root = entityEntry.Entity as IAggregateRoot;
                    foreach (var @event in root.GetUncommittedEvents())
                    {
                        DomainEvents.Raise(@event);
                    }
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

        private void ProcessAfterSaveChanges()
        {
            foreach (var entityEntry in this.ChangeTracker.Entries())
            {
                entityEntry.State = EntityState.Detached;
            }
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            PublishUncommitedDomainEvents();
            var affectedRows = base.SaveChangesAsync(cancellationToken);
            ProcessAfterSaveChanges();
            return affectedRows;
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
        {
            var affectedRows=  base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            ProcessAfterSaveChanges();
            return affectedRows;
        }
    }
}