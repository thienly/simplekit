using System.Threading.Tasks;
using MongoDB.Driver;
using Pluralize.NET;
using SimpleKit.Domain.Entities;
using SimpleKit.Infrastructure.Repository.MongoDb.Abstractions;

namespace SimpleKit.Infrastructure.Repository.MongoDb.Implementations
{
    public class MongoDbContext : IMongoDbContext
    {
        public IMongoClient Client { get; }
        public IMongoDatabase Database { get; }
        
        public MongoDbContext(string connectionString)
        {
            var mongoUrl = new MongoUrl(connectionString);
            Client = new MongoClient(mongoUrl);
            Database = Client.GetDatabase(mongoUrl.DatabaseName,new MongoDatabaseSettings());
        }
        public MongoDbContext(string connectionString, string databaseName)
        {
            var mongoUrl = new MongoUrl(connectionString);
            Client = new MongoClient(mongoUrl);
            Database = Client.GetDatabase(databaseName);
        }

        public MongoDbContext(string connection, MongoDatabaseSettings settings) : this(connection)
        {
            var mongoUrl = new MongoUrl(connection);
            Database = Client.GetDatabase(mongoUrl.DatabaseName, settings);
        }

        public MongoDbContext(MongoDbContext dbContext)
        {
            Client = dbContext.Client;
            Database = dbContext.Database;
        }
        public Task DropCollection<TDocument>() where TDocument : IAggregateRoot
        {
            return Database.DropCollectionAsync(GetCollectionName<TDocument>());
        }

        public IMongoCollection<TDocument> GetCollection<TDocument>() where TDocument : IAggregateRoot
        {
            return Database.GetCollection<TDocument>(GetCollectionName<TDocument>());
        }

        public string GetCollectionName<TDocument>()
        {
            var pluralizer = new Pluralizer();
            return pluralizer.Pluralize(typeof(TDocument).Name).ToLower();
        }
    }
}