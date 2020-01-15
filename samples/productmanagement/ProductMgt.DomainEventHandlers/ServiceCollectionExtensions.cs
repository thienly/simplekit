using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using ProductMgt.Domain.Events;
using SimpleKit.Domain.Events;

namespace ProductMgt.DomainEventHandlers
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDomainEventHandler(this IServiceCollection serviceCollection)
        {
            var callingAssembly = Assembly.Load("ProductMgt.DomainEventHandlers");
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