using System;
using Microsoft.Extensions.DependencyInjection;
using SimpleKit.Infrastructure.Repository.EfCore.Db;

namespace SimpleKit.Infrastructure.Repository.EfCore.SqlServer
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSimpleKitEfCoreSql<T>(this IServiceCollection collection)
        {
            collection.AddSingleton<IDbContextFactory, DbContextFactory>();
            collection.AddScoped<AppDbContext>(provider =>
            {
                AppDbContext dbContext = (AppDbContext)provider.GetService<IDbContextFactory>().Create(typeof(T));
                return dbContext;


            });
            collection.AddScoped(typeof(T),
                provider =>
                {
                    var dbContext = provider.GetService<IDbContextFactory>().Create(typeof(T));
                    return dbContext;
                });
            return collection;
        }
        public static IServiceCollection AddEfCoreSqlTemplate<T>(this IServiceCollection collection) where T: AppDbContext
        {
            return collection.AddDomainEventHandler()
                .AddSimpleKitEfCore()
                .AddSimpleKitEfCoreSql<T>();
        }
    }
}