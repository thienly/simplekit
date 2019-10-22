using System.Linq;
using SimpleKit.Domain.Entities;

namespace SimpleKit.Domain.Repositories
{
    public interface IQueryRepository<out TEntity> where TEntity : class, IAggregateRoot
    {
        IQueryable<TEntity> Queryable();
    }
}