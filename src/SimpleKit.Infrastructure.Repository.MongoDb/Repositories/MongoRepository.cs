using System.Threading.Tasks;
using SimpleKit.Domain.Entities;
using SimpleKit.Domain.Events;
using SimpleKit.Domain.Repositories;
using SimpleKit.Infrastructure.Repository.MongoDb.Abstractions;

namespace SimpleKit.Infrastructure.Repository.MongoDb.Repositories
{
    public class MongoRepository<TEntity, TKey> : IRepository<TEntity> where TEntity : AggregateRootWithId<TKey>
    {
        private readonly IBaseMongoRepositoryAdd _repositoryAdd;
        private readonly IBaseMongoRepositoryUpdate _repositoryUpdate;
        private readonly IBaseMongoRepositoryDelete _repositoryDelete;
        private IDomainEventDispatcher _domainEventDispatcher;

        public MongoRepository(IBaseMongoRepositoryAdd repositoryAdd, IBaseMongoRepositoryUpdate repositoryUpdate,
            IBaseMongoRepositoryDelete repositoryDelete, IDomainEventDispatcher domainEventDispatcher)
        {
            _repositoryAdd = repositoryAdd;
            _repositoryUpdate = repositoryUpdate;
            _repositoryDelete = repositoryDelete;
            _domainEventDispatcher = domainEventDispatcher;
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
                _domainEventDispatcher.Dispatch(@event);
            }
            entity.ClearDomainEvents();
        }
    }
}