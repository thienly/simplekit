using System;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.Extensions.DependencyInjection;
using Mongo2Go;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using SimpleKit.Domain.Entities;
using SimpleKit.Domain.Identity;
using SimpleKit.Domain.Repositories;
using SimpleKit.Infrastructure.Repository.MongoDb;
using SimpleKit.Infrastructure.Repository.MongoDb.Abstractions;
using SimpleKit.Infrastructure.Repository.MongoDb.Repositories;
using Test.SimpleKit.Base;
using Test.SimpleKit.Infrastructure.Repository.MongoDb.SeedDb;
using Xunit;
using Xunit.Abstractions;
using GuidGenerator = MongoDB.Bson.Serialization.IdGenerators.GuidGenerator;

namespace Test.SimpleKit.Infrastructure.Repository.MongoDb
{
    public class MongoIntegrationTest : SimpleKitTestBase 
    {
        internal static MongoDbRunner _runner;

        public MongoIntegrationTest()
        {
            
        }
        internal static void CreateConnection()
        {
            _runner = MongoDbRunner.Start(); 
        }
    }    
    public class TestRepository : MongoIntegrationTest, IDisposable
    {
        private IServiceProvider _serviceProvider;
        private ITestOutputHelper _testOutputHelper;
        public TestRepository()
        {
            
            CreateConnection();
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddMongoDb(new MongoDbOptions()
            {
                ConnectionString = _runner.ConnectionString,
                DatabaseName = "IntergrationTest"
            });
            _serviceProvider = serviceCollection.BuildServiceProvider();
            if (!BsonClassMap.IsClassMapRegistered(typeof(IdentityBase<Guid>)))
            {
                BsonClassMap.RegisterClassMap<IdentityBase<Guid>>(map =>
                {
                    map.SetIsRootClass(true);
                    map.MapIdMember(x => x.Id).SetIdGenerator(GuidGenerator.Instance);
                });
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(IdentityBase<int>)))
            {
                BsonClassMap<IdentityBase<int>>.RegisterClassMap<IdentityBase<int>>(map =>
                {
                    map.SetIsRootClass(true);
                    map.MapIdMember(x => x.Id).SetIdGenerator(new IntergerIdGenerator<Person>());
                });
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(Person)))
            {
                BsonClassMap<Person>.RegisterClassMap<Person>(map =>
                {
                    map.AutoMap();
                    map.MapField("_bankAccounts").SetElementName("BankAccounts");
                });
            }
        }

        [Fact]
        public async Task<Person> Test_if_can_insert_new_document_and_return_new_document_with_id()
        {
            var uow = _serviceProvider.GetService<IUnitOfWork>();
            var repository = uow.Repository<Person>();
            var person = Fixture.Build<Person>().FromFactory(() =>
            {
                var p = new Person("TestName");
                p.AddBankAccount(Fixture.Create<BankAccount>());
                p.RegisterAddress(Fixture.Create<Address>());
                return p;
            }).Create();
            var addAsync = await repository.AddAsync(person);
            Assert.NotEqual(addAsync.Id ,0);
            return addAsync;
        }

        [Fact]
        public async Task Test_if_can_update_document()
        {
            // insert stage
            var addedPerson = await Test_if_can_insert_new_document_and_return_new_document_with_id();
            addedPerson.AddBankAccount(Fixture.Create<BankAccount>());
            var uow = _serviceProvider.GetService<IUnitOfWork>();
            var repository = uow.Repository<Person>();
            var updatedPerson = await repository.UpdateAsync(addedPerson);
            var queryFactory = _serviceProvider.GetService<IQueryRepositoryFactory>();
            var query = queryFactory.Create<Person>();
            var filterDefinition = Builders<Person>.Filter.Eq(x => x.Id, updatedPerson.Id);
            var person = query.Find(filterDefinition).First();
            Assert.NotNull(person.PermenantAddress);
            Assert.NotNull(person.BankAccounts);
        }

        [Fact]
        public async Task Test_if_can_delete_an_existed_document()
        {
            var addedPerson = await Test_if_can_insert_new_document_and_return_new_document_with_id();
            var uow = _serviceProvider.GetService<IUnitOfWork>();
            var repository = uow.Repository<Person>();
            await repository.DeleteAsync(addedPerson);
            var queryFactory = _serviceProvider.GetService<IQueryRepositoryFactory>();
            var query = queryFactory.Create<Person>();
            var filterDefinition = Builders<Person>.Filter.Eq(x => x.Id, addedPerson.Id);
            var countDocuments = query.Find(filterDefinition).CountDocuments();
            Assert.True(countDocuments == 0);
        }

        public void Dispose()
        {
            var mongoDbContext = _serviceProvider.GetService<IMongoDbContext>();
            mongoDbContext.DropCollection<Person>();
        }
    }

    public class IntergerIdGenerator<TEntity> : IIdGenerator where TEntity: class, IAggregateRoot
    {
        public object GenerateId(object container, object document)
        {
            var collection = (IMongoCollection<TEntity>) container;
            var countDocuments = collection.Find(FilterDefinition<TEntity>.Empty).CountDocuments();
            return (int) countDocuments +1;
        }

        public bool IsEmpty(object id)
        {
            return (int)id == default;
        }
    }
}