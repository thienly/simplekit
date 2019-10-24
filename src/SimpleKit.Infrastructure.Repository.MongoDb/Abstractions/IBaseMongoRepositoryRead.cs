using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using SimpleKit.Domain.Entities;

namespace SimpleKit.Infrastructure.Repository.MongoDb.Abstractions
{
    public interface IBaseMongoRepositoryRead
    {
        IMongoQueryable<TEntity> Queryable<TEntity, TKey>() where TEntity : AggregateRootWithId<TKey>;

        IFindFluent<TEntity, TEntity> Find<TEntity,TKey>(FilterDefinition<TEntity> filter)
            where TEntity : AggregateRootWithId<TKey>;
        IFindFluent<TEntity, TEntity> Find<TEntity,TKey>(Expression<Func<TEntity,bool>> filter)
            where TEntity : AggregateRootWithId<TKey>;

        IAsyncCursor<TResult> AsyncCursor<TEntity, TKey,TResult>(PipelineDefinition<TEntity,TResult> pipelineDefinition)
            where TEntity : AggregateRootWithId<TKey>;

        Task<IAsyncCursor<TResult>> AsyncCursorAsync<TEntity, TKey,TResult>(PipelineDefinition<TEntity,TResult> pipelineDefinition)
            where TEntity : AggregateRootWithId<TKey>;
    }
}