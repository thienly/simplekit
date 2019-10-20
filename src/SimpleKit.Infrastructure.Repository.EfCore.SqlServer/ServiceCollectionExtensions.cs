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
            collection.AddScoped<AppDbContext>(provider => provider.GetService<IDbContextFactory>().Create(type));
            collection.AddScoped(type,
                provider => provider.GetService<IDbContextFactory>().Create(type));
            return collection;
        }
    }
}