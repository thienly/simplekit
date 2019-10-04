using System.Threading.Tasks;
using SimpleKit.Domain.Entities;

namespace SimpleKit.Domain.Repositories
{
    public interface IRepository<TEntity> where TEntity : IAggregateRoot
    {
        Task<TEntity> AddAsync(TEntity entity);
        Task<TEntity> UpdateAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
    }
}