using Microsoft.EntityFrameworkCore;
using SimpleKit.Domain;
using SimpleKit.Domain.Entities;

namespace SimpleKit.Infrastructure.Repository.EfCore.Repository
{
    public class QueryRepositoryFactory : IQueryRepositoryFactory
    {
        private DbContext _dbContext;

        public QueryRepositoryFactory(DbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public IQueryRepository<TEntity> Create<TEntity>() where TEntity : class, IAggregateRoot
        {
            return new QueryRepository<TEntity>(_dbContext);
        }
    }
}