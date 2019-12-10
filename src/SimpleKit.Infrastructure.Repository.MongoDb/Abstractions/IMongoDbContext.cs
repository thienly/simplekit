using System.Threading.Tasks;
using MongoDB.Driver;
using SimpleKit.Domain.Entities;

namespace SimpleKit.Infrastructure.Repository.MongoDb.Abstractions
{
    public interface IMongoDbContext
    {
        IMongoClient Client { get; }
        IMongoDatabase Database { get; }
        Task DropCollection<TDocument>() where TDocument : IAggregateRoot;
        IMongoCollection<TDocument> GetCollection<TDocument>() where TDocument: IAggregateRoot;
        string GetCollectionName<TEntity>();
    }
}