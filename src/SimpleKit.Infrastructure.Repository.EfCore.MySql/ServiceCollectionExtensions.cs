using System;
using Microsoft.Extensions.DependencyInjection;
using SimpleKit.Infrastructure.Repository.EfCore.Db;

namespace SimpleKit.Infrastructure.Repository.EfCore.MySql
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMySQL(this IServiceCollection collection, Type dbContextType )
        {
            collection.AddSingleton<IDbContextFactory, DbContextFactory>();
            collection.AddScoped(provider => provider.GetService<IDbContextFactory>().Create(dbContextType));
            collection.AddScoped(dbContextType,
                provider => provider.GetService<IDbContextFactory>().Create(dbContextType));
            return collection;
        }
    }
}