using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using SimpleKit.Domain.Entities;

namespace SimpleKit.Domain.Repositories
{
    public interface IUnitOfWork: IDisposable
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        int SaveChanges();
        IRepository<TEntity> Repository<TEntity>() where TEntity : class, IAggregateRoot;
    }
}