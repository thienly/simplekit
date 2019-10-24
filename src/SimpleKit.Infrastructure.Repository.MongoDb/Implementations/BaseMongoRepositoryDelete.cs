using System.Threading.Tasks;
using SimpleKit.Domain.Entities;
using SimpleKit.Infrastructure.Repository.MongoDb.Abstractions;
using SimpleKit.Infrastructure.Repository.MongoDb.Db;

namespace SimpleKit.Infrastructure.Repository.MongoDb.Implementations
{
    public class BaseMongoRepositoryDelete : IBaseMongoRepositoryDelete
    {
        private MongoDbDeleter _mongoDbDeleter;

        public BaseMongoRepositoryDelete(MongoDbDeleter mongoDbDeleter)
        {
            _mongoDbDeleter = mongoDbDeleter;
        }

        public bool Delete<TDocument, TKey>(TDocument document) where TDocument : AggregateRootWithId<TKey>
        {
            return _mongoDbDeleter.DeleteOne<TDocument, TKey>(document);
        }

        public Task<bool> DeleteAsync<TDocument, TKey>(TDocument document) where TDocument : AggregateRootWithId<TKey>
        {
            return _mongoDbDeleter.DeleteOneAsync<TDocument, TKey>(document);
        }
    }
}