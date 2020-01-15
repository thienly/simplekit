using System;
using Microsoft.Extensions.DependencyInjection;
using SimpleKit.Infrastructure.Repository.EfCore.Db;

namespace SimpleKit.Infrastructure.Repository.EfCore.SqlServer
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSimpleKitEfCoreSql(this IServiceCollection collection,
            Type type)
        {
            collection.AddSingleton<IDbContextFactory, DbContextFactory>();
            collection.AddScoped<AppDbContext>(provider =>
            {
                AppDbContext dbContext = (AppDbContext)provider.GetService<IDbContextFactory>().Create(type);
                return dbContext;


            });
            collection.AddScoped(type,
                provider =>
                {
                    var dbContext = provider.GetService<IDbContextFactory>().Create(type);
                    return dbContext;
                });
            return collection;
        }
        public static IServiceCollection AddEfCoreSqlTemplate(this IServiceCollection collection, Type typeOfDbContext)
        {
            return collection.AddSimpleKitEfCore().AddSimpleKitEfCoreSql(typeOfDbContext);
        }
    }
}