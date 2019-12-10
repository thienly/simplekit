using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using Castle.Windsor;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Test.SimpleKit.Base;
using Test.SimpleKit.Repository.EfCore.Base.SeedDb;
using Xunit.Abstractions;

namespace Test.SimpleKit.Repository.EfCore.Base
{
    public class TestWithSqlite: IDisposable
    {
        private IFixture _fixture;
        protected SuiteDbContext SuiteDbContext;
        private string _connectionString = "Data Source=:memory:;";
        private SqliteConnection _connection;
        protected IWindsorContainer _container = new WindsorContainer();
        private ITestOutputHelper _testOutputHelper;
        protected TestWithSqlite(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _fixture = new Fixture();
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _fixture.Customize(new AutoMoqCustomization());
            
            var optionsBuilder = new DbContextOptionsBuilder<SuiteDbContext>();
            _connection = new SqliteConnection(_connectionString);
            _connection.Open();
            var dbContextOptions = optionsBuilder.UseSqlite(_connection)
                .UseLoggerFactory(new LoggerFactory(new[] {new UnitTestLoggerProvider(_testOutputHelper)}))
                .EnableSensitiveDataLogging()
                .Options;
            
            SuiteDbContext = new SuiteDbContext(dbContextOptions);
            SuiteDbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            SuiteDbContext.ChangeTracker.AutoDetectChangesEnabled = false;
            SuiteDbContext.Database.EnsureCreated(); 
            
        }

        protected IFixture Fixture => _fixture;
        public void Dispose()
        {
            _connection.Close();
    
            SuiteDbContext.Dispose();
            
        }

        protected void SeedData()
        {
            var persons = Fixture.Build<Person>().FromSeed(p =>
            {
                var person = new Person("T1");
                person.RegisterAddress(new Address()
                {
                    Street = "Street1",
                    AddressNumber = "AddressNumber1",
                    Ward = "Ward1"
                });
                person.AddBankAccount(new BankAccount("ACB"));
                return person;
            }).CreateMany<Person>(100);
            SuiteDbContext.Set<Person>().AddRange(persons);
            SuiteDbContext.SaveChanges();
        }
    }

    
}