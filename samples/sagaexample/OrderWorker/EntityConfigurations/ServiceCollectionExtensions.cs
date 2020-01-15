using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrderWorker.Domains;
using SimpleKit.Domain.Events;
using SimpleKit.Infrastructure.Repository.EfCore;
using SimpleKit.Infrastructure.Repository.EfCore.Db;
using SimpleKit.Infrastructure.Repository.EfCore.SqlServer;

namespace OrderWorker.EntityConfigurations
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
                .AddOrderDbContext(sqlServerOptions);
            serviceCollection.AddScoped<IDbContextTransaction>((x) =>
                x.GetService<OrderDbContext>().Database.BeginTransaction());
            return serviceCollection;
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
        private static IServiceCollection AddOrderDbContext(this IServiceCollection serviceCollection,EfCoreSqlServerOptions sqlServerOptions)
        {
            serviceCollection.AddDbContext<OrderDbContext>(builder =>
                builder.UseSqlServer(sqlServerOptions.MainDbConnectionString));
            return serviceCollection;
        }

    }
    
}