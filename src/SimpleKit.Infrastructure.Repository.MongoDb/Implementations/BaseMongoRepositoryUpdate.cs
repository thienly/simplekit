using System.Threading.Tasks;
using SimpleKit.Domain.Entities;
using SimpleKit.Infrastructure.Repository.MongoDb.Abstractions;
using SimpleKit.Infrastructure.Repository.MongoDb.Db;

namespace SimpleKit.Infrastructure.Repository.MongoDb.Implementations
{
    public class BaseMongoRepositoryUpdate : IBaseMongoRepositoryUpdate
    {
        private MongoDbUpdater _mongoDbUpdater;

        public BaseMongoRepositoryUpdate(MongoDbUpdater mongoDbUpdater)
        {
            _mongoDbUpdater = mongoDbUpdater;
        }

        public bool Update<TDocument, TKey>(TDocument document) where TDocument : AggregateRootWithId<TKey>
        {
            return _mongoDbUpdater.UpdateOne<TDocument, TKey>(document);
        }

        public Task<bool> UpdateAsync<TDocument, TKey>(TDocument document) where TDocument : AggregateRootWithId<TKey>
        {
            return _mongoDbUpdater.UpdateOneAsync<TDocument, TKey>(document);
        }
    }
}