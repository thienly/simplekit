using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using SimpleKit.Infrastructure.Bus.RabbitMq.Interfaces;

namespace SimpleKit.Infrastructure.Bus.RabbitMq
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRabbitMq(this IServiceCollection serviceCollection, RabbitMQOptions options, params Assembly[] eventHandlersAssemblies)
        {
            serviceCollection.AddTransient<RabbitMQOptions>(provider => options);
            serviceCollection.AddSingleton<ISubscriptionManager, SubscriptionManager>();
            serviceCollection.AddSingleton<IRabbitMQChannelFactory, RabbitMqChannelFactory>();
            serviceCollection.AddSingleton<IRabbitMQMemoryBus, RabbitMQMemoryBus>();
            serviceCollection.AddSingleton<IntegrationEventHandlerFactory>(provider => (t) => provider.GetService(t));
            serviceCollection.RegisterGenericType(typeof(IIntegrationEventHandler<>), ServiceLifetime.Transient,eventHandlersAssemblies);
            serviceCollection.RegisterAssembly(typeof(IDynamicIntegrationEventHandler), ServiceLifetime.Transient,eventHandlersAssemblies);
            serviceCollection.AddSingleton<IIntegrationEventAggregation, IntegrationEventAggregation>();
            return serviceCollection;
        }

        private static IServiceCollection RegisterAssembly(this IServiceCollection serviceCollection, Type serviceType,
            ServiceLifetime serviceLifetime = ServiceLifetime.Transient, params Assembly[] scanAssemblies)
        {
            var typeInfos = scanAssemblies.SelectMany(x => x.DefinedTypes).Where(t=> typeof(IDynamicIntegrationEventHandler).IsAssignableFrom(t));
            foreach (var typeInfo in typeInfos)
            {
                serviceCollection.Add(new ServiceDescriptor(serviceType,typeInfo,serviceLifetime));
                serviceCollection.Add(new ServiceDescriptor(typeInfo,typeInfo,serviceLifetime));
            }
            return serviceCollection;
        }
        private static IServiceCollection RegisterGenericType(this IServiceCollection serviceCollection, Type serviceType,
            ServiceLifetime serviceLifetime = ServiceLifetime.Transient, params Assembly[] scanAssemblies)
        {
            var typeInfos = scanAssemblies.SelectMany(x=> x.DefinedTypes).Where(t => t.ImplementedInterfaces.Any(t=> t.IsGenericType && t.GetGenericTypeDefinition() == serviceType));
            foreach (var typeInfo in typeInfos)
            {
                var interfaceServiceType = typeInfo.ImplementedInterfaces.First(x=>x.GetGenericTypeDefinition() == serviceType);
                serviceCollection.Add(new ServiceDescriptor(interfaceServiceType, typeInfo, serviceLifetime));
                serviceCollection.Add(new ServiceDescriptor(typeInfo, typeInfo, serviceLifetime));
            }
            return serviceCollection;
        }
    }
}