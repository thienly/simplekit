using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleKit.Infrastructure.Repository.EfCore.Db;

namespace SimpleKit.Infrastructure.Repository.EfCore.MySql
{
    public class DbContextFactory : IDbContextFactory
    {
        private IOptions<MySQLOptions> _options;
        private ILoggerFactory _loggerFactory; 
        public DbContextFactory(IOptions<MySQLOptions> options, ILoggerFactory loggerFactory)
        {
            _options = options;
            _loggerFactory = loggerFactory;
        }
        public T Create<T>() where T : AppDbContext
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseMySql(_options.Value.MainDbConnectionString)
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .EnableDetailedErrors()
                .UseLoggerFactory(_loggerFactory);
            // need to improve
            var dbContext =(T)Activator.CreateInstance(typeof(T),optionsBuilder.Options);
            return dbContext;
        }

        public AppDbContext Create(Type type)
        {
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseMySql(_options.Value.MainDbConnectionString)
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .EnableDetailedErrors()
                .UseLoggerFactory(_loggerFactory);
            var dbContext =Activator.CreateInstance(type,optionsBuilder.Options);
            return dbContext as AppDbContext;
        }
    }
}