using System.Collections.Generic;
using System.Threading.Tasks;
using SimpleKit.Domain.Entities;

namespace SimpleKit.Infrastructure.Repository.MongoDb.Abstractions
{
    public interface IBaseMongoRepositoryAdd
    {
        void Add<TDocument, TKey>(TDocument document) where TDocument : AggregateRootWithId<TKey>;
        Task AddAsync<TDocument, TKey>(TDocument document) where TDocument : AggregateRootWithId<TKey>;
        void AddMany<TDocument, TKey>(IEnumerable<TDocument> documents) where TDocument : AggregateRootWithId<TKey>;
        Task AddManyAsync<TDocument, TKey>(IEnumerable<TDocument> documents) where TDocument : AggregateRootWithId<TKey>;
    }
}