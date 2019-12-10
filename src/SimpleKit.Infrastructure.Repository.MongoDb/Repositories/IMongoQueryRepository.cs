using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using SimpleKit.Domain.Entities;

namespace SimpleKit.Infrastructure.Repository.MongoDb.Repositories
{
    public interface IMongoQueryRepository<TEntity> where TEntity: class, IAggregateRoot
    {
        IMongoQueryable<TEntity> Queryable();
        IFindFluent<TEntity,TEntity> Find(FilterDefinition<TEntity> filterDefinition);
        IAsyncCursor<TResult> AsyncCursor<TResult>(PipelineDefinition<TEntity,TResult> pipelineDefinition);
        Task<IAsyncCursor<TResult>> AsyncCursorAsync<TResult>(PipelineDefinition<TEntity,TResult> pipelineDefinition);
    }
}