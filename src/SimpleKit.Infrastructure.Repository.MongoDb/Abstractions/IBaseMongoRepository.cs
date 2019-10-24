using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using SimpleKit.Domain.Entities;

namespace SimpleKit.Infrastructure.Repository.MongoDb.Abstractions
{
    public interface IBaseMongoRepository : IBaseMongoRepositoryAdd, IBaseMongoRepositoryUpdate, IBaseMongoRepositoryDelete
    {
        Task<List<TDocument>> GetPaginatedAsync<TDocument>(Expression<Func<TDocument, bool>> filter, int skipNumber = 0,
            int takeNumber = 50)
            where TDocument : IAggregateRoot;

        Task<TDocument> GetAndUpdateOne<TDocument>(FilterDefinition<TDocument> filter,
            UpdateDefinition<TDocument> update, FindOneAndUpdateOptions<TDocument, TDocument> options)
            where TDocument : IAggregateRoot;

        IMongoQueryable<TEntity> Queryable<TEntity, TKey>() where TEntity : AggregateRootWithId<TKey>;
    }
}