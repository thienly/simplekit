using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SimpleKit.Domain;
using SimpleKit.Infrastructure.Repository.EfCore.Repository;

namespace SimpleKit.Infrastructure.Repository.EfCore.Extensions
{
    public static class RepositoryExtensions
    {
        public static Task<TEntity> FindById<TEntity>(this IQueryRepository<TEntity> queryRepository,Guid id, Expression<Func<TEntity,object>> includes )
            where TEntity : class, IAggregateRoot
        {
            var queryable = queryRepository.Queryable();
            var includableQueryable = queryable.Include(includes);
            //return await includableQueryable.SingleOrDefaultAsync(e => e.Id == id);
            return Task.FromResult(default(TEntity));
        }  
    }
}