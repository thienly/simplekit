using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using SimpleKit.Domain.Entities;
using SimpleKit.Infrastructure.Repository.MongoDb.Abstractions;

namespace SimpleKit.Infrastructure.Repository.MongoDb.Repositories
{
    public class MongoQueryRepository<TEntity,TKey> : IQueryRepository<TEntity> where TEntity: AggregateRootWithId<TKey>
    {
        private IBaseMongoRepositoryRead _repositoryRead;

        public MongoQueryRepository(IBaseMongoRepositoryRead repositoryRead)
        {
            _repositoryRead = repositoryRead;
        }

        public IMongoQueryable<TEntity> Queryable()
        {
            return _repositoryRead.Queryable<TEntity,TKey>();
        }

        public IFindFluent<TEntity, TEntity> Find(FilterDefinition<TEntity> filterDefinition)
        {
            return _repositoryRead.Find<TEntity, TKey>(filterDefinition);
        }

        public IAsyncCursor<TResult> AsyncCursor<TResult>(PipelineDefinition<TEntity, TResult> pipelineDefinition)
        {
            return _repositoryRead.AsyncCursor<TEntity, TKey, TResult>(pipelineDefinition);
        }

        public Task<IAsyncCursor<TResult>> AsyncCursorAsync<TResult>(PipelineDefinition<TEntity, TResult> pipelineDefinition)
        {
            return _repositoryRead.AsyncCursorAsync<TEntity, TKey, TResult>(pipelineDefinition);
        }
    }
}