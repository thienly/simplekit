using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using SimpleKit.Domain.Repositories;
using SimpleKit.Infrastructure.Repository.MongoDb;
using SimpleKit.Infrastructure.Repository.MongoDb.Abstractions;
using SimpleKit.Infrastructure.Repository.MongoDb.Implementations;
using SimpleKit.Infrastructure.Repository.MongoDb.Repositories;
using Test.SimpleKit.Domain.SeedDb;
using Test.SimpleKit.Infrastructure.Repository.MongoDb.Models;
using Xunit;

namespace Test.SimpleKit.Infrastructure.Repository.MongoDb
{
    
    public class TestServiceProvider
    {
        private IServiceProvider _serviceProvider;
        public TestServiceProvider()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddMongoDb(new MongoDbOptions
            {
                ConnectionString = "mongodb://admin:new@10.0.19.103:27017/admin"
            });
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }
        [Fact]
        public async Task Test_If_service_collection_working_correctly()
        {
            var mongoDbContext = _serviceProvider.GetService<IMongoDbContext>();
            var listDatabaseNamesAsync = await mongoDbContext.Client.ListDatabaseNamesAsync();
            var list = listDatabaseNamesAsync.ToList();
            Assert.True(list.Count > 0);
        }

        [Fact]
        public void Test_If_Unit_of_work_can_create_repository()
        {
            var unitOfWork = _serviceProvider.GetService<IUnitOfWork>();
            var repository = unitOfWork.Repository<Person>();
            var baseRepository = unitOfWork.Repository<Book>();
            Assert.NotNull(repository);
            Assert.NotNull(baseRepository);
        }

        [Fact]
        public void Test_If_can_resolve_MongoQueryRepository()
        {
            var factory = _serviceProvider.GetService<IQueryRepositoryFactory>();
            var mongoRepositoryQuery = factory.Create<Person>();
            var mongoRepositoryBookQuery = factory.Create<Book>();
            Assert.NotNull(mongoRepositoryQuery);
            Assert.NotNull(mongoRepositoryBookQuery);
        }
        
    }
}