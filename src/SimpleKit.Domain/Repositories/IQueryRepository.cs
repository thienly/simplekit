using SimpleKit.Domain.Entities;

namespace SimpleKit.Domain.Repositories
{
    public interface IQueryRepository<TEntity> where TEntity : class, IAggregateRoot
    {
    }
}