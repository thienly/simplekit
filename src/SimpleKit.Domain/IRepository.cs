using System.Threading;
using System.Threading.Tasks;

namespace SimpleKit.Domain
{
    public interface IRepository<TEntity> where TEntity : IAggregateRoot
    {
        Task<TEntity> AddAsync(TEntity entity);
        Task<TEntity> UpdateAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
    }

    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        int SaveChanges();
        IRepository<TEntity> Repository<TEntity>() where TEntity : class, IAggregateRoot;
    }
}