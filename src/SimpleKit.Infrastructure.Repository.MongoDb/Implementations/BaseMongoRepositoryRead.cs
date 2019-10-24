using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using SimpleKit.Domain.Entities;
using SimpleKit.Infrastructure.Repository.MongoDb.Abstractions;
using SimpleKit.Infrastructure.Repository.MongoDb.Db;

namespace SimpleKit.Infrastructure.Repository.MongoDb.Implementations
{
    public class BaseMongoRepositoryRead : IBaseMongoRepositoryRead
    {
        private MongoDbReader _mongoDbReader;

        public BaseMongoRepositoryRead(MongoDbReader mongoDbReader)
        {
            _mongoDbReader = mongoDbReader;
        }

        public IMongoQueryable<TEntity> Queryable<TEntity, TKey>() where TEntity : AggregateRootWithId<TKey>
        {
            return _mongoDbReader.GetQuery<TEntity, TKey>();
        }

        public IFindFluent<TEntity, TEntity> Find<TEntity, TKey>(FilterDefinition<TEntity> filter) where TEntity : AggregateRootWithId<TKey>
        {
            return _mongoDbReader.Find<TEntity, TKey>(filter);
        }

        public IFindFluent<TEntity, TEntity> Find<TEntity, TKey>(Expression<Func<TEntity, bool>> filter) where TEntity : AggregateRootWithId<TKey>
        {
            return _mongoDbReader.Find<TEntity, TKey>(filter);
        }

        public IAsyncCursor<TResult> AsyncCursor<TEntity, TKey, TResult>(PipelineDefinition<TEntity, TResult> pipelineDefinition) where TEntity : AggregateRootWithId<TKey>
        {
            return _mongoDbReader.AsyncCursor<TEntity, TKey, TResult>(pipelineDefinition);
        }

        public Task<IAsyncCursor<TResult>> AsyncCursorAsync<TEntity, TKey, TResult>(PipelineDefinition<TEntity, TResult> pipelineDefinition) where TEntity : AggregateRootWithId<TKey>
        {
            return _mongoDbReader.AsyncCursorAsync<TEntity, TKey, TResult>(pipelineDefinition);
        }
    }
}