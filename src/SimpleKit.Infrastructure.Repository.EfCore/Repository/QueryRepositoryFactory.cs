using System;
using Microsoft.Extensions.DependencyInjection;
using SimpleKit.Domain.Entities;
using SimpleKit.Domain.Repositories;

namespace SimpleKit.Infrastructure.Repository.EfCore.Repository
{
    public class QueryRepositoryFactory : IQueryRepositoryFactory
    {
        private readonly IServiceProvider _serviceProvider;
        public QueryRepositoryFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IQueryRepository<TEntity> Create<TEntity>() where TEntity : class, IAggregateRoot
        {
            return _serviceProvider.GetService<IQueryRepository<TEntity>>();
        }
    }
}