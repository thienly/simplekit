using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using ProductMgt.Infrastructure.UserContext;
using SimpleKit.Domain.Events;
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
            serviceCollection.AddScoped<IDbContextFactory, ProductMgtDbContextFactory>();
            serviceCollection.AddScoped(typeof(ProductMgtDbContext),
                provider =>
                {
                    var dbContextFactory = provider.GetService<IDbContextFactory>();
                    var dbContext =  dbContextFactory.Create(typeof(ProductMgtDbContext));
                    dbContext.DomainEventDispatcher = provider.GetService<IDomainEventDispatcher>();
                    return dbContext;
                });
            serviceCollection.AddScoped<AppDbContext>(provider =>
            {
                var dbContext = provider.GetService<ProductMgtDbContext>();
                return dbContext;
            });
            serviceCollection.AddSingleton<IUserContextFactory, UserContextFactory>();
            return serviceCollection;
        }

    }
}