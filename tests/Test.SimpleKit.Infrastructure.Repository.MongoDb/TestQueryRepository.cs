using System;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using SimpleKit.Infrastructure.Repository.MongoDb;
using SimpleKit.Infrastructure.Repository.MongoDb.Abstractions;
using SimpleKit.Infrastructure.Repository.MongoDb.Repositories;
using Test.SimpleKit.Infrastructure.Repository.MongoDb.Models;
using Xunit;

namespace Test.SimpleKit.Infrastructure.Repository.MongoDb
{
    public class TestQueryRepository
    {
        private IServiceProvider _serviceProvider;
        private MongoDbSeeder _mongoDbSeeder;
        public TestQueryRepository()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddMongoDb(new MongoDbOptions()
            {
                ConnectionString = "mongodb://admin:new@10.0.19.103:27017/admin"
            });
            _serviceProvider = serviceCollection.BuildServiceProvider();
            _mongoDbSeeder = new MongoDbSeeder(_serviceProvider.GetService<IMongoDbContext>());
        }

        [Fact]
        public void Test_if_Find_fluent_working_good()
        {
            using (_mongoDbSeeder.Seed<Book>(100))
            {
                var queryFactory = _serviceProvider.GetService<IQueryRepositoryFactory>();
                var mongoQueryRepository = queryFactory.Create<Book>();
                var filterDefinition = Builders<Book>.Filter.Empty;
                var countDocuments = mongoQueryRepository.Find(filterDefinition).CountDocuments();
                Assert.True(countDocuments == 100);
            }
        }
    }
}