using System;
using System.Data;
using System.Data.Common;
using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Test.SimpleKit.Repository.EfCore.SeedDb;

namespace Test.SimpleKit.Repository.EfCore
{
    public class TestWithSqlite: IDisposable
    {
        private IFixture _fixture;
        protected SuiteDbContext SuiteDbContext;
        private string _connectionString = "Data Source=:memory:;";
        private SqliteConnection _connection;
        protected TestWithSqlite()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _fixture.Customize(new AutoMoqCustomization());
            var optionsBuilder = new DbContextOptionsBuilder<SuiteDbContext>();
            _connection = new SqliteConnection(_connectionString);
            _connection.Open();
            var dbContextOptions = optionsBuilder.UseSqlite(_connection).Options;
            SuiteDbContext = new SuiteDbContext(dbContextOptions);
            SuiteDbContext.Database.EnsureCreated();
        }

        protected IFixture Fixture => _fixture;
        public void Dispose()
        {
            _connection.Close();
            SuiteDbContext.Dispose();
        }
    }
}