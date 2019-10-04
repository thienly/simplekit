using SimpleKit.Domain;
using SimpleKit.Domain.Entities;

namespace SimpleKit.Infrastructure.Repository.EfCore.Repository
{
    public interface IQueryRepositoryFactory
    {
        IQueryRepository<TEntity> Create<TEntity>() where TEntity :  class, IAggregateRoot;
    }
}