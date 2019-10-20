using System.Linq;
using Microsoft.EntityFrameworkCore;
using SimpleKit.Domain;
using SimpleKit.Domain.Entities;
using SimpleKit.Domain.Repositories;
using SimpleKit.Infrastructure.Repository.EfCore.Db;

namespace SimpleKit.Infrastructure.Repository.EfCore.Repository
{
    public class EfQueryRepository<TEntity> : IQueryRepository<TEntity> where TEntity : class, IAggregateRoot
    {
        private readonly AppDbContext _dbContext;

        public EfQueryRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IQueryable<TEntity> Queryable()
        {
            return _dbContext.Set<TEntity>();
        }
    }
}