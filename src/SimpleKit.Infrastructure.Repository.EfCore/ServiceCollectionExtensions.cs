using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using SimpleKit.Domain.Events;
using SimpleKit.Domain.Repositories;
using SimpleKit.Infrastructure.Repository.EfCore.Db;
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
            collection.AddScoped<IDbContextTransaction>(provider =>
            {
                var appDbContext = provider.GetService<AppDbContext>();
                return appDbContext.Database.BeginTransaction();
            });
            return collection;
        }
        public static IServiceCollection AddDomainEventHandler(this IServiceCollection serviceCollection)
        {
            var callingAssembly = Assembly.GetExecutingAssembly();
            var types = callingAssembly.GetTypes()
                .Where(x =>
                {
                    return x.GetInterfaces().Any(i =>
                               i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>)) && !x.IsAbstract;
                }).ToList();
            foreach (var implementedType in types)
            {
                var interfaces = implementedType.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>));
                foreach (var @interface in interfaces)
                {
                    serviceCollection.AddScoped(@interface, implementedType);
                }
            }
            
            serviceCollection.AddScoped<EventHandlerFactory>(provider =>
                t => provider.GetServices(t));
            serviceCollection.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
            return serviceCollection;
        }
    }
}