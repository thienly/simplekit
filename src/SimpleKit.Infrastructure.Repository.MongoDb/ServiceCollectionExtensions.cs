using Microsoft.Extensions.DependencyInjection;
using SimpleKit.Domain.Repositories;
using SimpleKit.Infrastructure.Repository.MongoDb.Abstractions;
using SimpleKit.Infrastructure.Repository.MongoDb.Db;
using SimpleKit.Infrastructure.Repository.MongoDb.Implementations;
using SimpleKit.Infrastructure.Repository.MongoDb.Repositories;

namespace SimpleKit.Infrastructure.Repository.MongoDb
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMongoDb(this IServiceCollection serviceCollection, MongoDbOptions options)
        {
            serviceCollection.AddScoped<IMongoDbContext>(provider => new MongoDbContext(options.ConnectionString));
            serviceCollection.AddScoped<MongoDbReader, MongoDbReader>();
            serviceCollection.AddScoped<MongoDbCreator, MongoDbCreator>();
            serviceCollection.AddScoped<MongoDbUpdater, MongoDbUpdater>();
            serviceCollection.AddScoped<MongoDbDeleter, MongoDbDeleter>();
            serviceCollection.AddScoped<IBaseMongoRepositoryAdd, BaseMongoRepositoryAdd>();
            serviceCollection.AddScoped<IBaseMongoRepositoryRead, BaseMongoRepositoryRead>();
            serviceCollection.AddScoped<IBaseMongoRepositoryUpdate, BaseMongoRepositoryUpdate>();
            serviceCollection.AddScoped<IBaseMongoRepositoryDelete, BaseMongoRepositoryDelete>();
            serviceCollection.AddSingleton<IQueryRepositoryFactory, QueryRepositoryFactory>();
            serviceCollection.AddScoped<IUnitOfWork, MongoDbUnitOfWork>();
            serviceCollection.AddScoped(typeof(MongoRepository<,>), typeof(MongoRepository<,>));
            serviceCollection.AddScoped(typeof(MongoQueryRepository<,>), typeof(MongoQueryRepository<,>));
            serviceCollection.AddScoped<RepositoryFactory>(provider => (t) =>
            {
                return provider.GetService(t);
            });
            return serviceCollection;
        }
    }
}