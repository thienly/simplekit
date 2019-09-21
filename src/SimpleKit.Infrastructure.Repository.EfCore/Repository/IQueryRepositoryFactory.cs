using SimpleKit.Domain;

namespace SimpleKit.Infrastructure.Repository.EfCore.Repository
{
    public interface IQueryRepositoryFactory
    {
        IQueryRepository<TEntity> Create<TEntity>() where TEntity :  class, IAggregateRoot;
    }
}