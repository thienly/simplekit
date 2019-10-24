using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SimpleKit.Domain.Repositories;
using SimpleKit.Infrastructure.Repository.EfCore;
using SimpleKit.Infrastructure.Repository.EfCore.Extensions;
using SimpleKit.Infrastructure.Repository.EfCore.SqlServer;
using Test.DatabaseGenerator.DbContext;
using Test.SimpleKit.Domain.SeedDb;
using Test.SimpleKit.Repository.EfCore.Base;
using Xunit;
using Xunit.Abstractions;

namespace Test.SimpleKit.Repository.EfCore
{
    public class Test_EfRepositoryWithSqlServer : TestWithSqlServer
    {
        private ITestOutputHelper _testOutputHelper;
        private IServiceProvider _serviceProvider;

        public Test_EfRepositoryWithSqlServer(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddOptions<EfCoreSqlServerOptions>()
                .Configure(options =>
                    options.MainDbConnectionString = "Server=.;Database=EfCoreTest;User Id=sa;Password=Test!234");
            serviceCollection.AddLogging();
            serviceCollection.AddScoped<IDbContextTransaction>((x) =>
                x.GetService<SuiteDbContext>().Database.BeginTransaction());
            serviceCollection.AddSingleton<ILoggerFactory>(new LoggerFactory(new[]
                {new UnitTestLoggerProvider(_testOutputHelper)}));
            
            serviceCollection.AddSimpleKitEfCore()
                .AddSimpleKitEfCoreSql(typeof(SuiteDbContext));
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        [Fact]
        public async Task Test_if_db_context_component_registered_ok()
        {
            var suiteDbContext = _serviceProvider.GetService<SuiteDbContext>();
            var canConnect = await suiteDbContext.Database.CanConnectAsync();
            Assert.NotNull(suiteDbContext);
            Assert.True(canConnect);
        }

        [Fact]
        public async Task Test_if_query_factory_working_properly()
        {
            var suiteDbContext = _serviceProvider.GetService<SuiteDbContext>();
            var personRepository = _serviceProvider.GetService<IQueryRepository<Person>>();

            using (SeedData(suiteDbContext))
            {
                var persons = await personRepository.Find(p => p.Id > 0, new List<Expression<Func<Person, object>>>()
                {
                    p => p.PermenantAddress
                });
                Assert.True(persons != null);
                Assert.True(persons.All(x => x.PermenantAddress != null));
            }
        }

        [Fact]
        public async Task Test_if_can_add_add_entity_to_database()
        {
            var unitOfWork = _serviceProvider.GetService<IUnitOfWork>();
            var repository = unitOfWork.Repository<Person>();
            var person = await repository.AddAsync(Fixture.Build<Person>().FromFactory(() =>
            {
                var p = new Person("Name");
                p.RegisterAddress(Fixture.Create<Address>());
                return p;
            }).Create<Person>());
            using (var scope = _serviceProvider.CreateScope())
            {
                var suiteDbContext = _serviceProvider.GetService<SuiteDbContext>();
                var p = suiteDbContext.Set<Person>().Find(person.Id);
                Assert.Equal( p ,person);
            }

        }
    }
}