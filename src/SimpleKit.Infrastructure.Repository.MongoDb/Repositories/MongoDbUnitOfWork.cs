using System;
using System.Threading;
using System.Threading.Tasks;
using SimpleKit.Domain.Entities;
using SimpleKit.Domain.Repositories;

namespace SimpleKit.Infrastructure.Repository.MongoDb.Repositories
{
    public delegate object RepositoryFactory(Type type);

    public class MongoDbUnitOfWork : IUnitOfWork
    {
        private RepositoryFactory _repositoryFactory;

        public MongoDbUnitOfWork(RepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(1);
        }

        public int SaveChanges()
        {
            return 1;
        }

        public IRepository<TEntity> Repository<TEntity>() where TEntity : class, IAggregateRoot
        {
            Type baseT = typeof(TEntity);
            while (baseT.BaseType != typeof(object))
            {
                baseT = baseT.BaseType;
            }
            var keyType = baseT.GetGenericArguments()[0];
            return _repositoryFactory(typeof(MongoRepository<,>).MakeGenericType(typeof(TEntity),keyType)) as IRepository<TEntity>;
        }

        public void Dispose()
        {
        }
    }
}