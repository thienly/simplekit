//using System;
//using Microsoft.Extensions.DependencyInjection;
//using SimpleKit.Domain.Repositories;
//using SimpleKit.Infrastructure.Repository.EfCore.Db;
//using SimpleKit.Infrastructure.Repository.EfCore.SqlServer;
//
//namespace SimpleKit.Infrastructure.Repository.EfCore.MySql
//{
//    public static class ServiceCollectionExtensions
//    {
//        public static IServiceCollection AddEfCoreSql(this IServiceCollection collection)
//        {
//            collection.AddSingleton<IDbContextFactory, DbContextFactory>();
//            collection.AddScoped<AppDbContext>(provider => provider.GetService<IDbContextFactory>().Create());
//            return collection;
//        }
//    }
//}