using System;
using SimpleKit.Domain.Entities;
using SimpleKit.Domain.Repositories;

namespace SimpleKit.Infrastructure.Repository.MongoDb.Repositories
{
    public interface IQueryRepositoryFactory
    {
        IQueryRepository<TEntity> Create<TEntity>() where TEntity : class, IAggregateRoot;
    }

    public class QueryRepositoryFactory : IQueryRepositoryFactory
    {
        private IServiceProvider _serviceProvider;

        public QueryRepositoryFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IQueryRepository<TEntity> Create<TEntity>() where TEntity : class,IAggregateRoot
        {
            Type baseT = typeof(TEntity);
            while (baseT.BaseType != typeof(object))
            {
                baseT = baseT.BaseType;
            }
            var tKey = baseT.GetGenericArguments()[0];
            return (IQueryRepository<TEntity>) _serviceProvider.GetService(
                typeof(MongoQueryRepository<,>).MakeGenericType(typeof(TEntity),tKey));
        }
    }
}