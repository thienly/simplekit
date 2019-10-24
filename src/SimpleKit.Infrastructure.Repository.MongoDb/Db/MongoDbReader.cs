using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using SimpleKit.Domain.Entities;
using SimpleKit.Infrastructure.Repository.MongoDb.Abstractions;
using SimpleKit.Infrastructure.Repository.MongoDb.Implementations;

namespace SimpleKit.Infrastructure.Repository.MongoDb.Db
{
    public class MongoDbReader : MongoDbBase
    {
        private readonly IMongoDbContext _dbContext;

        public MongoDbReader(IMongoDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public MongoDbReader(string connectionString) : base(connectionString)
        {
            _dbContext = new MongoDbContext(connectionString);
        }

        public TDocument GetById<TDocument, TKey>(TKey value) where TDocument : AggregateRootWithId<TKey>
        {
            var mongoCollection = _dbContext.GetCollection<TDocument>();
            var filterDefinition = Builders<TDocument>.Filter.Eq("Id",value);
            return mongoCollection.Find(filterDefinition).SingleOrDefault();
        }

        public Task<TDocument> GetByIdAsync<TDocument, TKey>(TKey value) where TDocument : AggregateRootWithId<TKey>
        {
            var mongoCollection = _dbContext.GetCollection<TDocument>();
            var filterDefinition = Builders<TDocument>.Filter.Eq("Id", value);
            return mongoCollection.Find(filterDefinition).SingleOrDefaultAsync();
        }

        public IFindFluent<TDocument, TDocument> Find<TDocument, TKey>(
            FilterDefinition<TDocument> filterDefinition) where TDocument : AggregateRootWithId<TKey>
        {
            return _dbContext.GetCollection<TDocument>().Find(filterDefinition);
        }
        public IFindFluent<TDocument, TDocument> Find<TDocument,TKey>(Expression<Func<TDocument, bool>> filter)
            where TDocument : AggregateRootWithId<TKey>
        {
            return _dbContext.GetCollection<TDocument>().Find(filter);
        }

        public Task<IAsyncCursor<TResult>> AsyncCursorAsync<TDocument, TKey, TResult>(
            PipelineDefinition<TDocument, TResult> pipelineDefinition) where TDocument : AggregateRootWithId<TKey>
        {
            return _dbContext.GetCollection<TDocument>().AggregateAsync(pipelineDefinition);
        }

        public IAsyncCursor<TResult> AsyncCursor<TDocument, TKey, TResult>(
            PipelineDefinition<TDocument, TResult> pipelineDefinition) where TDocument : AggregateRootWithId<TKey>
        {
            return _dbContext.GetCollection<TDocument>().Aggregate(pipelineDefinition);
        }
        
        public TDocument MinBy<TDocument, TKey, TField>(FilterDefinition<TDocument> filter,
            Expression<Func<TDocument, TField>> selector) where TDocument : AggregateRootWithId<TKey>
        {
            var collection = _dbContext.GetCollection<TDocument>();
            var sorting = Builders<TDocument>.Sort.Ascending(BuilderHelpers.ConvertExpression(selector));
            return collection.Find(filter).Sort(sorting).FirstOrDefault();
        }
        
        public Task<TDocument> MinByAsync<TDocument, TKey, TField>(FilterDefinition<TDocument> filter,
            Expression<Func<TDocument, TField>> selector) where TDocument : AggregateRootWithId<TKey>
        {
            var collection = _dbContext.GetCollection<TDocument>();
            var sorting = Builders<TDocument>.Sort.Ascending(BuilderHelpers.ConvertExpression(selector));
            return collection.Find(filter).Sort(sorting).FirstOrDefaultAsync();
        }

        public TDocument MaxBy<TDocument, TKey, TField>(FilterDefinition<TDocument> filter,
            Expression<Func<TDocument, TField>> selector) where TDocument : AggregateRootWithId<TDocument>
        {
            var collection = _dbContext.GetCollection<TDocument>();
            var sorting = Builders<TDocument>.Sort.Descending(BuilderHelpers.ConvertExpression(selector));
            return collection.Find(filter).Sort(sorting).FirstOrDefault();
        }
        
        public Task<TDocument> MaxByAsync<TDocument, TKey, TField>(FilterDefinition<TDocument> filter,
            Expression<Func<TDocument, TField>> selector) where TDocument : AggregateRootWithId<TDocument>
        {
            var collection = _dbContext.GetCollection<TDocument>();
            var sorting = Builders<TDocument>.Sort.Descending(BuilderHelpers.ConvertExpression(selector));
            return collection.Find(filter).Sort(sorting).FirstOrDefaultAsync();
        }

        public decimal SumBy<TDocument, TKey, TField>(Expression<Func<TDocument,bool>> filter,
            Expression<Func<TDocument, decimal>> selector) where TDocument: AggregateRootWithId<TKey>
        {
            var collection = _dbContext.GetCollection<TDocument>();
            return GetQuery<TDocument, TKey>().Where(filter).Sum(selector);
        }
        
        public Task<decimal> SumByAsync<TDocument, TKey, TField>(Expression<Func<TDocument,bool>> filter,
            Expression<Func<TDocument, decimal>> selector) where TDocument: AggregateRootWithId<TKey>
        {
            var collection = _dbContext.GetCollection<TDocument>();
            return GetQuery<TDocument, TKey>().Where(filter).SumAsync(selector);
        }

        public Task<List<TDocument>> GetAll<TDocument, Tkey>(FilterDefinition<TDocument> filterDefinition)
            where TDocument : AggregateRootWithId<Tkey>
        {
            var collection = _dbContext.GetCollection<TDocument>();
            return collection.Find(filterDefinition).ToListAsync();
        }
        public List<TDocument> GetAll<TDocument, Tkey>(Expression<Func<TDocument, bool>> filter) where TDocument: AggregateRootWithId<Tkey>
        {
            return GetQuery<TDocument, Tkey>().Where(filter).ToList();
        }
        
    }
}