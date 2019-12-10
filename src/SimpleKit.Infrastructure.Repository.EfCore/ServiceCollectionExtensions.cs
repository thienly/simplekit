using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using SimpleKit.Domain.Events;
using SimpleKit.Domain.Repositories;
using SimpleKit.Infrastructure.Repository.EfCore.Repository;

namespace SimpleKit.Infrastructure.Repository.EfCore
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSimpleKitEfCore(this IServiceCollection collection)
        {
            collection.AddScoped<IUnitOfWork, EfUnitOfWork>();
            collection.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            collection.AddScoped<RepositoryFactory>(provider => (t) => provider.GetService(t));
            collection.AddScoped<IQueryRepositoryFactory, QueryRepositoryFactory>();
            collection.AddScoped(typeof(IQueryRepository<>), typeof(EfQueryRepository<>));
            return collection;
        }
    }
}