using SimpleKit.Domain;
using SimpleKit.Domain.Entities;
using SimpleKit.Domain.Repositories;

namespace SimpleKit.Infrastructure.Repository.EfCore.Repository
{
    public interface IQueryRepositoryFactory
    {
        IQueryRepository<TEntity> Create<TEntity>() where TEntity :  class, IAggregateRoot;
    }
}