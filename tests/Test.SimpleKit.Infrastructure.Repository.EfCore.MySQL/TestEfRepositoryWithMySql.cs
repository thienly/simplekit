using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SimpleKit.Domain.Repositories;
using SimpleKit.Infrastructure.Repository.EfCore;
using SimpleKit.Infrastructure.Repository.EfCore.Extensions;
using SimpleKit.Infrastructure.Repository.EfCore.MySql;
using Test.SimpleKit.Base;
using Test.SimpleKit.Infrastructure.Repository.EfCore.MySQL.Base;
using Test.SimpleKit.Infrastructure.Repository.EfCore.MySQL.Base.SeedDb;
using Xunit;
using Xunit.Abstractions;

namespace Test.SimpleKit.Infrastructure.Repository.EfCore.MySQL
{
    public class TestEfRepositoryWithMySql : TestWithMySQL
    {
        private ITestOutputHelper _testOutputHelper;
        private IServiceProvider _serviceProvider;

        public TestEfRepositoryWithMySql(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddOptions<MySQLOptions>()
                .Configure(options =>
                    options.MainDbConnectionString =
                        "server=10.0.19.103;port=3306;user=root;password=new;database=TestDB");
            serviceCollection.AddLogging();
            serviceCollection.AddScoped((x) =>
                x.GetService<SuiteMySQLDbContext>().Database.BeginTransaction());
            serviceCollection.AddSingleton<ILoggerFactory>(new LoggerFactory(new[]
                {new UnitTestLoggerProvider(_testOutputHelper)}));
            serviceCollection.AddSimpleKitEfCore()
                .AddMySQL(typeof(SuiteMySQLDbContext));
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        [Fact]
        public async Task Test_if_DbContext_configuration_properly()
        {
            var suiteMySqlDbContext = _serviceProvider.GetService<SuiteMySQLDbContext>();
            suiteMySqlDbContext.Database.EnsureDeleted();
            var ensureCreated = suiteMySqlDbContext.Database.EnsureCreated();
            ensureCreated.Should().BeTrue();
            suiteMySqlDbContext.Database.EnsureDeleted();
        }

        [Fact]
        public async Task Test_if_query_factory_working_properly()
        {
            var suiteDbContext = _serviceProvider.GetService<SuiteMySQLDbContext>();
            using (InitializeDatabase(suiteDbContext))
            {
                var personRepository = _serviceProvider.GetService<IQueryRepository<Person>>();
                using (SeedData(suiteDbContext))
                {
                    var persons = await personRepository.Find(p => p.Id > 0,
                        new List<Expression<Func<Person, object>>>()
                        {
                            p => p.PermenantAddress
                        });
                    Assert.True(persons != null);
                    Assert.True(persons.All(x => x.PermenantAddress != null));
                }
            }
        }

        [Fact]
        public async Task Test_if_can_add_add_entity_to_database()
        {
            var dbContext = _serviceProvider.GetService<SuiteMySQLDbContext>();
            using (InitializeDatabase(dbContext))
            {
                var unitOfWork = _serviceProvider.GetService<IUnitOfWork>();
                var repository = unitOfWork.Repository<Person>();
                var person = await repository.AddAsync(Fixture.Build<Person>().FromFactory(() =>
                {
                    var p = new Person("Name");
                    p.RegisterAddress(Fixture.Create<Address>());
                    return p;
                }).Create<Person>());
                var p = dbContext.Set<Person>().Find(person.Id);
                Assert.Equal(p, person);
            }
        }
    }
}