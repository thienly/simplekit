using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;
using SimpleKit.Domain.Entities;
using SimpleKit.Infrastructure.Repository.MongoDb.Abstractions;

namespace SimpleKit.Infrastructure.Repository.MongoDb.Db
{
    public class MongoDbUpdater
    {
        private readonly IMongoDbContext _dbContext;

        public MongoDbUpdater(IMongoDbContext dbContext)
        {
            _dbContext = dbContext;
        }
            
        public async Task<bool> UpdateOneAsync<TDocument,TKey>(TDocument document) where TDocument: AggregateRootWithId<TKey>
        {
            var mongoCollection = _dbContext.GetCollection<TDocument>();
            var filter = BuilderHelpers.RequiredFilter<TDocument,TKey>(document);
            var replaceOneResult = await mongoCollection.ReplaceOneAsync(filter, document);
            return replaceOneResult.ModifiedCount == 1;
        }

        public bool UpdateOne<TDocument,TKey>(TDocument document) where TDocument : AggregateRootWithId<TKey>
        {
            var mongoCollection = _dbContext.GetCollection<TDocument>();
            var filter = BuilderHelpers.RequiredFilter<TDocument,TKey>(document);
            var replaceOneResult = mongoCollection.ReplaceOne(filter, document);
            return replaceOneResult.ModifiedCount == 1;
        }

        public async Task<bool> UpdateOneAsync<TDocument, TKey>(TDocument document,
            UpdateDefinition<TDocument> updateDefinition) where TDocument : AggregateRootWithId<TKey>
        {
            var mongoCollection = _dbContext.GetCollection<TDocument>();
            var filter = BuilderHelpers.RequiredFilter<TDocument,TKey>(document);
            var replaceOneResult = await mongoCollection.UpdateOneAsync(filter, updateDefinition, new UpdateOptions()
            {
                IsUpsert = true
            });
            return replaceOneResult.ModifiedCount == 1;
        }

        public bool UpdateOne<TDocument,TKey>(TDocument document, UpdateDefinition<TDocument> updateDefinition) where TDocument: AggregateRootWithId<TKey>
        {
            var mongoCollection = _dbContext.GetCollection<TDocument>();
            var filter = BuilderHelpers.RequiredFilter<TDocument,TKey>(document);
            var replaceOneResult = mongoCollection.UpdateOne(filter, updateDefinition, new UpdateOptions()
            {
                IsUpsert = true
            });
            return replaceOneResult.ModifiedCount == 1;
        }

        public async Task<bool> UpdateOneAsync<TDocument, TKey, TField>(TDocument document,
            Expression<Func<TDocument, TField>> field, TField value) where TDocument : AggregateRootWithId<TKey>
        {
            var mongoCollection = _dbContext.GetCollection<TDocument>();
            var filter = BuilderHelpers.RequiredFilter<TDocument,TKey>(document);
            var updateBuilder = Builders<TDocument>.Update.Set(field, value);
            var replaceOneResult = await mongoCollection.UpdateOneAsync(filter, updateBuilder, new UpdateOptions()
            {
                IsUpsert = true
            });
            return replaceOneResult.ModifiedCount == 1;
        }
        public bool UpdateOne<TDocument, TKey, TField>(TDocument document,
            Expression<Func<TDocument, TField>> field, TField value) where TDocument : AggregateRootWithId<TKey>
        {
            var mongoCollection = _dbContext.GetCollection<TDocument>();
            var filter = BuilderHelpers.RequiredFilter<TDocument,TKey>(document);
            var updateBuilder = Builders<TDocument>.Update.Set(field, value);
            var replaceOneResult = mongoCollection.UpdateOne(filter, updateBuilder, new UpdateOptions()
            {
                IsUpsert = true
            });
            return replaceOneResult.ModifiedCount == 1;
        }

        public async Task<bool> UpdateManyAsync<TDocument, TKey>(IEnumerable<TDocument> documents, UpdateDefinition<TDocument> updateDefinition) where TDocument: AggregateRootWithId<TKey>
        {
            var collection = _dbContext.GetCollection<TDocument>();


            var updateResult = collection.UpdateMany(BuilderHelpers.RequiredFilter<TDocument, TKey>(documents),
                updateDefinition, new UpdateOptions()
                {
                    IsUpsert = true
                });
            return updateResult.ModifiedCount == documents.Count();
        }
    }
}