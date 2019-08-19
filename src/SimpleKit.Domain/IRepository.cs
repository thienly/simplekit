using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleKit.Domain
{
    public interface IRepository<TEntity> where TEntity : AggregateRootWithId<Guid>
    {
    }

    public interface IRepositoryWithId<TEntity, TId> where TEntity : AggregateRootWithId<TId>
    {
        Task<TEntity> FindByIdAsync(TId id);
        Task<ICollection<TEntity>> FindAsync(ISpecification<TEntity> specification);
        TEntity Insert(TEntity entity);
        Task<ICollection<TEntity>> InsertAsync(IReadOnlyCollection<TEntity> entities, CancellationToken token);
        Task<TEntity> UpdateAsync(TEntity entity,CancellationToken cancellationToken);
        Task<TEntity> Update(TEntity entity);
        Task<TEntity> UpdateAsync(TEntity entity, Expression<Func<TEntity, object>> columns,CancellationToken cancellationToken);
        Task Delete(TId key);
        Task Delete(TEntity entity);
    }

    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}