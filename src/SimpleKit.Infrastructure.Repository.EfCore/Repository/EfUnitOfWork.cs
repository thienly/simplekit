using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SimpleKit.Domain;

namespace SimpleKit.Infrastructure.Repository.EfCore.Repository
{
    /// <summary>
    /// The other call for this is Repository for command handler
    /// </summary>
    
    public class EfUnitOfWork : IUnitOfWork
    {
        private readonly DbContext _dbContext;
        private ConcurrentDictionary<Type, object> _repositories = new ConcurrentDictionary<Type, object>();

        public EfUnitOfWork(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }

        public int SaveChanges()
        {
            return _dbContext.SaveChanges();
        }

        public IRepository<TEntity> Repository<TEntity>() where TEntity : class, IAggregateRoot
        {
            if (_repositories.ContainsKey(typeof(TEntity)))
            {
                return _repositories[typeof(TEntity)] as IRepository<TEntity>;
            }
            var repository = new EfRepository<TEntity>(_dbContext);
            _repositories.TryAdd(typeof(TEntity), repository);
            return repository;
        }
    }
}