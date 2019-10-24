using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using Castle.Windsor;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Test.DatabaseGenerator.DbContext;
using Test.SimpleKit.Domain.SeedDb;
using Xunit.Abstractions;
using ILogger = Microsoft.Extensions.Logging.ILogger;

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

    public class UnitTestLoggerProvider : ILoggerProvider
    {
        private ITestOutputHelper _testOutputHelper;

        public UnitTestLoggerProvider(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        public void Dispose()
        {
            _testOutputHelper = null;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new UnitTestLogger(_testOutputHelper);
        }
    }
    
    public class UnitTestLogger : ILogger, IDisposable
    {
        private ITestOutputHelper _testOutputHelper;

        public UnitTestLogger(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var s = formatter(state,exception);
            _testOutputHelper.WriteLine(s);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return new UnitTestLogger(_testOutputHelper);
        }

        public void Dispose()
        {
            _testOutputHelper = null;
        }
    }
}