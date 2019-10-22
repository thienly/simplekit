using System.Threading.Tasks;
using MongoDB.Driver;
using SimpleKit.Domain.Entities;
using SimpleKit.Domain.Repositories;

namespace SimpleKit.Infrastructure.Repository.MongoDb.Repositories
{
    public interface IQueryRepositoryFactory
    {
        
    }

    public interface IMongoRepositoryQuery<TEntity, TKey> : IQueryRepository<TEntity>
        where TEntity : AggregateRootWithId<TKey>
    {
        IFindFluent<TEntity,TResult> Find<TResult>(FilterDefinition<TEntity> filterDefinition);
        Task<IFindFluent<TEntity,TResult>> FindAsync<TResult>(FilterDefinition<TEntity> filterDefinition);
        IAsyncCursor<TEntity> AsyncCursor(FilterDefinition<TEntity> filterDefinition);
        Task<IAsyncCursor<TEntity>> AsyncCursorAsync(FilterDefinition<TEntity> filterDefinition);
    }
}