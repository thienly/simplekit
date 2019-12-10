using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProductMgt.Infrastructure.UserContext;
using SimpleKit.Infrastructure.Repository.EfCore;
using SimpleKit.Infrastructure.Repository.EfCore.Db;
using SimpleKit.Infrastructure.Repository.EfCore.SqlServer;

namespace ProductMgt.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection serviceCollection,
            EfCoreSqlServerOptions sqlServerOptions)
        {
            serviceCollection.AddOptions<EfCoreSqlServerOptions>().Configure(options =>
                options.MainDbConnectionString = sqlServerOptions.MainDbConnectionString);
            serviceCollection
                .AddSimpleKitEfCore()
                .AddProductMgtDbContext();
            serviceCollection.AddScoped<IDbContextTransaction>((x) =>
                x.GetService<ProductMgtDbContext>().Database.BeginTransaction());
            return serviceCollection;
        }
        private static IServiceCollection AddProductMgtDbContext(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IDbContextFactory, ProductMgtDbContextFactory>();
            serviceCollection.AddScoped(typeof(ProductMgtDbContext),
                provider => provider.GetService<IDbContextFactory>().Create(typeof(ProductMgtDbContext)));
            serviceCollection.AddScoped<AppDbContext>(provider => provider.GetService<ProductMgtDbContext>());
            serviceCollection.AddSingleton<IUserContextFactory, UserContextFactory>();
            return serviceCollection;
        }
    }

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