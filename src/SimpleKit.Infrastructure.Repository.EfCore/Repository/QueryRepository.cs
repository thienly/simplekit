using System.Linq;
using Microsoft.EntityFrameworkCore;
using SimpleKit.Domain;
using SimpleKit.Domain.Entities;

namespace SimpleKit.Infrastructure.Repository.EfCore.Repository
{
    public class QueryRepository<TEntity> : IQueryRepository<TEntity> where TEntity : class, IAggregateRoot
    {
        private readonly DbContext _dbContext;

        public QueryRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IQueryable<TEntity> Queryable()
        {
            return _dbContext.Set<TEntity>();
        }
    }
}