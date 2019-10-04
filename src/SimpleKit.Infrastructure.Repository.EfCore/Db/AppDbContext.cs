using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using SimpleKit.Domain;
using SimpleKit.Domain.Entities;
using SimpleKit.Domain.Events;

namespace SimpleKit.Infrastructure.Repository.EfCore.Db
{
    public class AppDbContext : DbContext
    {
        private ILogger<AppDbContext> _logger;
        private IDbContextTransaction _transaction;
        public AppDbContext(ILogger<AppDbContext> logger, IDbContextTransaction transaction)
        {
            _logger = logger;
            _transaction = transaction;
        }

        public override int SaveChanges()
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
            return base.SaveChanges();
        }
    }
}