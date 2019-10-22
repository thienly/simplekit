using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleKit.Infrastructure.Repository.EfCore.Db;

namespace SimpleKit.Infrastructure.Repository.EfCore.SqlServer
{
    public class DbContextFactory : IDbContextFactory
    {
        private IOptions<EfCoreSqlServerOptions> _options;
        private ILoggerFactory _loggerFactory;
        public DbContextFactory(IOptions<EfCoreSqlServerOptions> options, ILoggerFactory loggerFactory)
        {
            _options = options;
            _loggerFactory = loggerFactory;
        }
        public T Create<T>() where T: AppDbContext
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(_options.Value.MainDbConnectionString)
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .EnableDetailedErrors()
                .UseLoggerFactory(_loggerFactory);
            var dbContext =(T)Activator.CreateInstance(typeof(T),optionsBuilder.Options);
            return dbContext;
        }

        public AppDbContext Create(Type type)
        {
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseSqlServer(_options.Value.MainDbConnectionString)
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .EnableDetailedErrors()
                .UseLoggerFactory(_loggerFactory);
            var dbContext =Activator.CreateInstance(type,optionsBuilder.Options);
            return dbContext as AppDbContext;
        }
    }
}