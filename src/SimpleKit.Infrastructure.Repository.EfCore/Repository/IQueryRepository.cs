using System.Linq;
using SimpleKit.Domain;

namespace SimpleKit.Infrastructure.Repository.EfCore.Repository
{
    public interface IQueryRepository<TEntity> where TEntity: class, IAggregateRoot
    {
        IQueryable<TEntity> Queryable();
    }
}