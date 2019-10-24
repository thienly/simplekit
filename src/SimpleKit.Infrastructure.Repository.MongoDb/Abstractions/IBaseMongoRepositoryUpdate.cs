using System.Threading.Tasks;
using SimpleKit.Domain.Entities;

namespace SimpleKit.Infrastructure.Repository.MongoDb.Abstractions
{
    public interface IBaseMongoRepositoryUpdate
    {
        bool Update<TDocument, TKey>(TDocument document) where TDocument : AggregateRootWithId<TKey>;
        Task<bool> UpdateAsync<TDocument, TKey>(TDocument document) where TDocument : AggregateRootWithId<TKey>;
    }
}