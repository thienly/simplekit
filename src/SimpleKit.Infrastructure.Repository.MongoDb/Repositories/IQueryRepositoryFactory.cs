using System;
using SimpleKit.Domain.Entities;
using SimpleKit.Domain.Repositories;

namespace SimpleKit.Infrastructure.Repository.MongoDb.Repositories
{
    public interface IQueryRepositoryFactory
    {
        IMongoQueryRepository<TEntity> Create<TEntity>() where TEntity : class, IAggregateRoot;
    }

    public class QueryRepositoryFactory : IQueryRepositoryFactory
    {
        private IServiceProvider _serviceProvider;

        public QueryRepositoryFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IMongoQueryRepository<TEntity> Create<TEntity>() where TEntity : class,IAggregateRoot
        {
            Type baseT = typeof(TEntity);
            while (baseT.BaseType != typeof(object))
            {
                baseT = baseT.BaseType;
            }
            var tKey = baseT.GetGenericArguments()[0];
            return (IMongoQueryRepository<TEntity>) _serviceProvider.GetService(
                typeof(MongoQueryRepository<,>).MakeGenericType(typeof(TEntity),tKey));
        }
    }
}