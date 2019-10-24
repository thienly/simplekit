using System.Collections.Generic;
using System.Threading.Tasks;
using SimpleKit.Domain.Entities;
using SimpleKit.Infrastructure.Repository.MongoDb.Abstractions;
using SimpleKit.Infrastructure.Repository.MongoDb.Db;

namespace SimpleKit.Infrastructure.Repository.MongoDb.Implementations
{
    public class BaseMongoRepositoryAdd : IBaseMongoRepositoryAdd
    {
        private MongoDbCreator _mongoDbCreator;

        public BaseMongoRepositoryAdd(MongoDbCreator mongoDbCreator)
        {
            _mongoDbCreator = mongoDbCreator;
        }

        public void Add<TDocument, TKey>(TDocument document) where TDocument : AggregateRootWithId<TKey>
        {
            _mongoDbCreator.AddOne<TDocument,TKey>(document);
        }

        public Task AddAsync<TDocument, TKey>(TDocument document) where TDocument : AggregateRootWithId<TKey>
        {
            return _mongoDbCreator.AddOneAsync<TDocument,TKey>(document);
        }

        public void AddMany<TDocument, TKey>(IEnumerable<TDocument> documents) where TDocument : AggregateRootWithId<TKey>
        {
            _mongoDbCreator.AddMany<TDocument,TKey>(documents);
        }

        public Task AddManyAsync<TDocument, TKey>(IEnumerable<TDocument> documents) where TDocument : AggregateRootWithId<TKey>
        {
            return _mongoDbCreator.AddManyAsync<TDocument, TKey>(documents);
        }
    }
}