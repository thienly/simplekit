using MongoDB.Driver;
using SimpleKit.Domain.Entities;
using SimpleKit.Domain.Repositories;

namespace Order.HotelService.Repositories
{
    public interface IMongoQueryRepository<TEntity> : IQueryRepository<TEntity> where TEntity : AggregateRootBase
    {
        IMongoCollection<TEntity> GetCollection();
    }
}