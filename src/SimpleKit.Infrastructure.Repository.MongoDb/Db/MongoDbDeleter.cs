using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SimpleKit.Domain.Entities;
using SimpleKit.Infrastructure.Repository.MongoDb.Abstractions;

namespace SimpleKit.Infrastructure.Repository.MongoDb.Db
{
    public class MongoDbDeleter
    {
        private readonly IMongoDbContext _mongoDbContext;

        public MongoDbDeleter(IMongoDbContext mongoDbContext)
        {
            _mongoDbContext = mongoDbContext;
        }
        
        public async Task<bool> DeleteOneAsync<TDocument, TKey>(TDocument document)
            where TDocument : AggregateRootWithId<TKey>
        {
            var collection = _mongoDbContext.GetCollection<TDocument>();
            var filter = BuilderHelpers.RequiredFilter<TDocument,TKey>(document);
            var deleteOne = await collection.DeleteOneAsync(filter);
            return deleteOne.DeletedCount == 1;
        }

        public bool DeleteOne<TDocument,TKey>(TDocument document) where TDocument: AggregateRootWithId<TKey>
        {
            var collection = _mongoDbContext.GetCollection<TDocument>();
            var filter = BuilderHelpers.RequiredFilter<TDocument, TKey>(document);
            var deleteResult = collection.DeleteOne(filter);
            return deleteResult.DeletedCount == 1;
        }

        public async Task<bool> DeleteManyAsync<TDocument, TKey>(IEnumerable<TDocument> documents)
            where TDocument : AggregateRootWithId<TKey>
        {
            var collection = _mongoDbContext.GetCollection<TDocument>();
            var filter = BuilderHelpers.RequiredFilter<TDocument, TKey>(documents);
            var deleteResult = collection.DeleteMany(filter);
            return deleteResult.DeletedCount == documents.Count();
        }
    }
}