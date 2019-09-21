using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SimpleKit.Domain;

namespace SimpleKit.Infrastructure.Repository.EfCore.Repository
{
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
            // we need id return immediately, need a real development from this point.  
            var entityEntry = _dbSet.Add(entity);
            return Task.FromResult(entityEntry.Entity);
        }

        public Task<TEntity> UpdateAsync(TEntity entity)
        {
            var entityEntry = _dbContext.Entry(entity);
            entityEntry.State = EntityState.Modified;
            return Task.FromResult(entity);
        }

        public Task DeleteAsync(TEntity entity)
        {
            var entityEntry = _dbContext.Entry(entity);
            entityEntry.State = EntityState.Deleted;
            return Task.FromResult(entity);
        }
    }
}