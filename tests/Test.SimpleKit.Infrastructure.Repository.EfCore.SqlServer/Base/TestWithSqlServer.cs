using System;
using System.Linq;
using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Test.SimpleKit.Base;
using Test.SimpleKit.Repository.EfCore.Base.SeedDb;
using Xunit.Abstractions;

namespace Test.SimpleKit.Repository.EfCore.Base
{
    public class TestWithSqlServer: SimpleKitTestBase
    {
        private string _connectionString = "Server=.;Database=EfCoreTest;User Id=sa;Password=Test!234";
        private ITestOutputHelper _testOutputHelper;
        protected TestWithSqlServer(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }
        public IDisposable SeedData(SuiteDbContext data)
        {
            var persons = Fixture.Build<Person>().FromFactory(() =>
            {
                var p = new Person("Name");
                p.RegisterAddress(Fixture.Create<Address>());
                return p;
            }).CreateMany<Person>(100).ToList();
            data.Set<Person>().AddRange(persons);
            data.SaveChanges();
            return new CleanupResource(data);
        }

        internal class CleanupResource : IDisposable
        {

            private SuiteDbContext _dbContext;

            public CleanupResource(SuiteDbContext dbContext)
            {
                _dbContext = dbContext;
            }

            public void Dispose()
            {
                _dbContext.Database.ExecuteSqlRaw("DELETE FROM Person");
            }
        }
    }
    
}