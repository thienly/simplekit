using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ProductMgt.Domain.Factories;

namespace ProductMgt.ApplicationService
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection serviceCollection)
        {
            var loadedAssembly = typeof(TransactionBehavior<,>).Assembly;
            serviceCollection.AddMediatR(cfg => cfg.AsScoped(), loadedAssembly);
            serviceCollection.AddScoped(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
            return serviceCollection;
        }

        public static IServiceCollection AddDomainFactory(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IProductFactory, ProductFactory>();
            return serviceCollection;
        }
    }
}