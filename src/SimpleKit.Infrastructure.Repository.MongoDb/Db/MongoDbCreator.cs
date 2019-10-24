using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using SimpleKit.Domain.Entities;
using SimpleKit.Infrastructure.Repository.MongoDb.Abstractions;

namespace SimpleKit.Infrastructure.Repository.MongoDb.Db
{
    public class MongoDbCreator
    {
        private readonly IMongoDbContext _mongoDbContext;
        public MongoDbCreator(IMongoDbContext mongoDbContext)
        {
            _mongoDbContext = mongoDbContext;
        }

        public void AddOne<TDocument, TKey>(TDocument document) where TDocument : AggregateRootWithId<TKey>
        {
            
            var mongoCollection = _mongoDbContext.GetCollection<TDocument>();
            mongoCollection.InsertOne(document);
        }
        public Task AddOneAsync<TDocument,TKey>(TDocument document) where TDocument : AggregateRootWithId<TKey>
        {
            var mongoCollection = _mongoDbContext.GetCollection<TDocument>();
            return mongoCollection.InsertOneAsync(document);
        }

        public Task AddMany<TDocument,TKey>(IEnumerable<TDocument> documents) where TDocument: AggregateRootWithId<TKey>
        {
            var mongoCollection = _mongoDbContext.GetCollection<TDocument>();
            return mongoCollection.InsertManyAsync(documents);
        }
        public Task AddManyAsync<TDocument, TKey>(IEnumerable<TDocument> documents) where TDocument : AggregateRootWithId<TKey>
        {
            var mongoCollection = _mongoDbContext.GetCollection<TDocument>();
            return mongoCollection.InsertManyAsync(documents);
        }
    }
}