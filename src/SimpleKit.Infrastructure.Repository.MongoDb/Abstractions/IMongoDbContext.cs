using System.Threading.Tasks;
using MongoDB.Driver;
using Pluralize.NET;
using SimpleKit.Domain.Entities;

namespace SimpleKit.Infrastructure.Repository.MongoDb.Abstractions
{
    public interface IMongoDbContext
    {
        IMongoClient Client { get; }
        IMongoDatabase Database { get; }
        Task DropCollection<TDocument>() where TDocument : IAggregateRoot;
        IMongoCollection<TDocument> GetCollection<TDocument>() where TDocument: IAggregateRoot;
    }

    public class MongoDbContext : IMongoDbContext
    {
        public IMongoClient Client { get; }
        public IMongoDatabase Database { get; }

        public MongoDbContext(string connectionString)
        {
            var mongoUrl = new MongoUrl(connectionString);
            Client = new MongoClient(mongoUrl);
            Database = Client.GetDatabase(mongoUrl.DatabaseName);
        }
        public Task DropCollection<TDocument>() where TDocument : IAggregateRoot
        {
            return Database.DropCollectionAsync(GetCollectionName<TDocument>());
        }

        public IMongoCollection<TDocument> GetCollection<TDocument>() where TDocument : IAggregateRoot
        {
            return Database.GetCollection<TDocument>(GetCollectionName<TDocument>());
        }

        private string GetCollectionName<TDocument>()
        {
            var pluralizer = new Pluralizer();
            return pluralizer.Pluralize(typeof(TDocument).Name).ToLower();
        }
    }
}