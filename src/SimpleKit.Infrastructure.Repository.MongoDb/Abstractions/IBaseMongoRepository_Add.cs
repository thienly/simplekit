using System.Collections.Generic;
using System.Threading.Tasks;
using SimpleKit.Domain.Entities;

namespace SimpleKit.Infrastructure.Repository.MongoDb.Abstractions
{
    public interface IBaseMongoRepository_Add
    {
        void Add<TDocument, TKey>(TDocument document) where TDocument : AggregateRootWithId<TKey>;
        Task AddAsync<TDocument, TKey>(TDocument document) where TDocument : AggregateRootWithId<TKey>;
        void AddMany<TDocument, Tkey>(IEnumerable<TDocument> documents) where TDocument : AggregateRootWithId<Tkey>;
        void AddManyAsync<TDocument, Tkey>(IEnumerable<TDocument> documents) where TDocument : AggregateRootWithId<Tkey>;
    }
}