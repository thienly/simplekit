using System;
using Microsoft.Extensions.DependencyInjection;

namespace SimpleKit.Infrastructure.Repository.EfCore.SqlServer
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSimpleKitEfCoreSql(this IServiceCollection collection,
            Type type)
        {
            collection.AddSingleton<IDbContextFactory, DbContextFactory>();
            collection.AddScoped(provider => provider.GetService<IDbContextFactory>().Create(type));
            collection.AddScoped(type,
                provider => provider.GetService<IDbContextFactory>().Create(type));
            return collection;
        }
        public static IServiceCollection AddEfCoreSqlTemplate(this IServiceCollection collection, Type typeOfDbContext)
        {
            return collection.AddSimpleKitEfCore().AddSimpleKitEfCoreSql(typeOfDbContext);
        }
    }
}