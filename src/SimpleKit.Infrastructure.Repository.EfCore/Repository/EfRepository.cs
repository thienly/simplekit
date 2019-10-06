using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SimpleKit.Domain;
using SimpleKit.Domain.Entities;
using SimpleKit.Domain.Events;
using SimpleKit.Domain.Repositories;

namespace SimpleKit.Infrastructure.Repository.EfCore.Repository
{
    /// <summary>
    /// All the method only focus on the aggregate root, all relationship is going to to with event 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class EfRepository<TEntity> : IRepository<TEntity> where TEntity : class,IAggregateRoot
    {
        private DbContext _dbContext;
        private DbSet<TEntity> _dbSet;
        public EfRepository(DbContext context)
        {
            _dbContext = context;
            _dbSet = _dbContext.Set<TEntity>();
        }
        
        public Task<TEntity> AddAsync(TEntity entity)
        {
            var entityEntry = _dbContext.Entry(entity);
            entityEntry.State = EntityState.Added;
            _dbContext.SaveChanges();
            ProcessUncommittedEvents(entity);
            return Task.FromResult(entityEntry.Entity);
        }

        public Task<TEntity> UpdateAsync(TEntity entity)
        {
            var entityEntry = _dbContext.Entry(entity);
            entityEntry.State = EntityState.Modified;
            ProcessUncommittedEvents(entity);
            return Task.FromResult(entity);
        }

        public Task DeleteAsync(TEntity entity)
        {
            var entityEntry = _dbContext.Entry(entity);
            entityEntry.State = EntityState.Deleted;
            ProcessUncommittedEvents(entity);
            return Task.FromResult(entity);
        }

        private void ProcessUncommittedEvents(TEntity entity)
        {
            foreach (var domainEvent in entity.GetUncommittedEvents())
            {
                DomainEvents.Raise(domainEvent);
            }
        }
    }
}