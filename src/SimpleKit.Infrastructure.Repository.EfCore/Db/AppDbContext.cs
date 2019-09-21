using Microsoft.EntityFrameworkCore;
using SimpleKit.Domain;
using SimpleKit.Domain.Events;

namespace SimpleKit.Infrastructure.Repository.EfCore.Db
{
    public class AppDbContext : DbContext
    {
        public override int SaveChanges()
        {
            // Dispatch domain events
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
            // Dispatch integration event. The integration event only fire if local data is successfully commited. 
        }
    }
}