using System.Threading;
using System.Threading.Tasks;
using SimpleKit.Domain.Entities;
using SimpleKit.Domain.Events;
using SimpleKit.Domain.Repositories;
using SimpleKit.Infrastructure.Repository.MongoDb.Abstractions;

namespace SimpleKit.Infrastructure.Repository.MongoDb.Repositories
{
    public delegate IRepository<TDocument> RepositoryFactory<TDocument,TKey>() where TDocument : AggregateRootWithId<TKey>;
    public class MongoDbUnitOfWork : IUnitOfWork
    {
        
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
            throw new System.NotImplementedException();
        }
    }

    public class MongoDbRepository<TEntity, TKey> : IRepository<TEntity> where TEntity : AggregateRootWithId<TKey>
    {
        private readonly IBaseMongoRepository_Add _repositoryAdd;
        private readonly IBaseMongoRepository_Update _repositoryUpdate;
        private readonly IBaseMongoRepository_Delete _repositoryDelete;

        public MongoDbRepository(IBaseMongoRepository_Add repositoryAdd, IBaseMongoRepository_Update repositoryUpdate,
            IBaseMongoRepository_Delete repositoryDelete)
        {
            _repositoryAdd = repositoryAdd;
            _repositoryUpdate = repositoryUpdate;
            _repositoryDelete = repositoryDelete;
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            await _repositoryAdd.AddAsync<TEntity,TKey>(entity);
            DomainDispatcher(entity);
            return entity;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            await _repositoryUpdate.UpdateAsync<TEntity, TKey>(entity);
            DomainDispatcher(entity);
            return entity;
        }

        public async Task DeleteAsync(TEntity entity)
        {
            await _repositoryDelete.DeleteAsync<TEntity, TKey>(entity);
            DomainDispatcher(entity);
        }

        private void DomainDispatcher(TEntity entity)
        {
            var readOnlyCollection = entity.GetUncommittedEvents();
            foreach (var @event in readOnlyCollection)
            {
                DomainEvents.Raise(@event);
            }
            entity.ClearDomainEvents();
        }
    }
}