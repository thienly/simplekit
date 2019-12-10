using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SimpleKit.Infrastructure.Bus.Kafka.Interfaces;

namespace SimpleKit.Infrastructure.Bus.Kafka
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddKafkaBus(this IServiceCollection serviceCollection, KafkaOptions options,ILoggerFactory loggerFactory, params Assembly[] eventHandlersAssemblies)
        {
            serviceCollection.AddTransient<KafkaOptions>(provider => options);
            serviceCollection.AddSingleton<ISubscriptionManager, SubscriptionManager>();
            serviceCollection.AddSingleton<IKafkaBus>(provider =>
            {
                var kafkaBus = new KafkaBus(provider.GetService<KafkaOptions>(),
                    provider.GetService<ISubscriptionManager>(),
                    provider.GetService<IntegrationEventHandlerFactory>());
                kafkaBus.Logger = loggerFactory.CreateLogger<KafkaBus>();
                return kafkaBus;
            });
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