using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using SimpleKit.Domain.Entities;
using SimpleKit.Infrastructure.Repository.MongoDb.Abstractions;
using SimpleKit.Infrastructure.Repository.MongoDb.Implementations;

namespace SimpleKit.Infrastructure.Repository.MongoDb.Db
{
    public class MongoDbBase
    {
        private IMongoDbContext _mongoDbContext;

        public MongoDbBase(IMongoDbContext mongoDbContext)
        {
            _mongoDbContext = mongoDbContext;
        }

        public MongoDbBase(string connectionString)
        {
            _mongoDbContext = new MongoDbContext(connectionString);
        }

        public IMongoQueryable<TDocument> GetQuery<TDocument, TKey>() where TDocument : AggregateRootWithId<TKey>
        {
            return _mongoDbContext.GetCollection<TDocument>().AsQueryable();
        }
    }
}