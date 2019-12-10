using System;
using AutoFixture;
using SimpleKit.Domain.Entities;
using SimpleKit.Infrastructure.Repository.MongoDb.Abstractions;
using Test.SimpleKit.Base;

namespace Test.SimpleKit.Infrastructure.Repository.MongoDb
{
    public class MongoDbSeeder : SimpleKitTestBase
    {
        private IMongoDbContext _mongoDbContext;

        public MongoDbSeeder(IMongoDbContext mongoDbContext)
        {
            _mongoDbContext = mongoDbContext;
        }

        public IDisposable Seed<TEntity>(int numberofDocuments) where TEntity : class, IAggregateRoot
        {
            var entities = Fixture.CreateMany<TEntity>(numberofDocuments);
            var mongoCollection = _mongoDbContext.GetCollection<TEntity>();
            mongoCollection.InsertMany(entities);
            return new Clear<TEntity>(_mongoDbContext);
        }

        internal class Clear<TEntity> : IDisposable where TEntity: class, IAggregateRoot
        {
            private IMongoDbContext _mongoDbContext;

            public Clear(IMongoDbContext mongoDbContext)
            {
                _mongoDbContext = mongoDbContext;
            }

            public void Dispose()
            {
                _mongoDbContext.Database.DropCollection(_mongoDbContext.GetCollectionName<TEntity>());
            }
        }
    }
}