using System.Threading.Tasks;
using SimpleKit.Domain.Entities;

namespace SimpleKit.Infrastructure.Repository.MongoDb.Abstractions
{
    public interface IBaseMongoRepository_Delete
    {
        bool Delete<TDocument, TKey>(TDocument document) where TDocument : AggregateRootWithId<TKey>;
        Task<bool> DeleteAsync<TDocument, TKey>(TDocument document) where TDocument : AggregateRootWithId<TKey>;
    }
}