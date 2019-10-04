using System.Linq;
using SimpleKit.Domain;
using SimpleKit.Domain.Entities;

namespace SimpleKit.Infrastructure.Repository.EfCore.Repository
{
    public interface IQueryRepository<TEntity> where TEntity: class, IAggregateRoot
    {
        IQueryable<TEntity> Queryable();
    }
}