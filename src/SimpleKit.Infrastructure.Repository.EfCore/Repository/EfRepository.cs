using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SimpleKit.Domain;
using SimpleKit.Domain.Entities;
using SimpleKit.Domain.Events;
using SimpleKit.Domain.Repositories;
using SimpleKit.Infrastructure.Repository.EfCore.Db;

namespace SimpleKit.Infrastructure.Repository.EfCore.Repository
{
    /// <summary>
    /// All the method only focus on the aggregate root, all relationship is going to to with event.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class EfRepository<TEntity> : IRepository<TEntity> where TEntity : class,IAggregateRoot
    {
        private AppDbContext _dbContext;
        private DbSet<TEntity> _dbSet;
        public EfRepository(AppDbContext context)
        {
            _dbContext = context;
            _dbSet = _dbContext.Set<TEntity>();
        }
        
        public virtual Task<TEntity> AddAsync(TEntity entity)
        {
            var entityEntry = _dbSet.Add(entity);
            _dbContext.SaveChanges();
            return Task.FromResult(entityEntry.Entity);
        }

        public virtual Task<TEntity> UpdateAsync(TEntity entity)
        {
            // We just modified the AR and rest will be handle via domain events
            var entityEntry = _dbContext.Entry(entity);
            entityEntry.State = EntityState.Modified;
            return Task.FromResult(entity);
        }

        public virtual Task DeleteAsync(TEntity entity)
        {
            //Consider again whether we should use Remove or set entity
            var entityEntry = _dbContext.Entry(entity);
            entityEntry.State = EntityState.Deleted;
            return Task.FromResult(entity);
        }
    }
}