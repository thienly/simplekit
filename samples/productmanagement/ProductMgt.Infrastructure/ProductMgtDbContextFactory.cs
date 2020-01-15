using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProductMgt.Infrastructure.UserContext;
using SimpleKit.Infrastructure.Repository.EfCore.Db;
using SimpleKit.Infrastructure.Repository.EfCore.SqlServer;

namespace ProductMgt.Infrastructure
{
    public class ProductMgtDbContextFactory : IDbContextFactory
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IOptions<EfCoreSqlServerOptions> _options;
        private readonly IUserContextFactory _userContextFactory;
        
        public ProductMgtDbContextFactory(IOptions<EfCoreSqlServerOptions> options, ILoggerFactory loggerFactory, IUserContextFactory userContextFactory)
        {
            _options = options;
            _loggerFactory = loggerFactory;
            _userContextFactory = userContextFactory;
            
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
            var dbContext =Activator.CreateInstance(type,optionsBuilder.Options) as ProductMgtDbContext;
            var userContext = _userContextFactory.Create();
            dbContext.UserContext = userContext;
            return dbContext;
        }

    }
}