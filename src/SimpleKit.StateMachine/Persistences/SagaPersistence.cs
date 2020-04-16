using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;

namespace SimpleKit.StateMachine.Persistences
{
    public class SagaPersistence : ISagaPersistence
    {
        private string _connectionString;
        private IMongoCollection<SagaStateProxy> _mongoCollection;

        public SagaPersistence(string connectionString)
        {
            _connectionString = connectionString;
            var url = new MongoUrl(_connectionString);
            var mongoClient = new MongoClient(url);
            _mongoCollection = mongoClient.GetDatabase(url.DatabaseName).GetCollection<SagaStateProxy>("sagamanager");
            
        }

        public SagaStateProxy Load(Guid sagaId)
        {
            var sagaStateProxy = _mongoCollection.AsQueryable().Where(x => x.SagaId == sagaId)
                .OrderByDescending(x => x.Version)
                .FirstOrDefault();
            return sagaStateProxy ?? new EmptySagaState();
        }

        public void Save(SagaStateProxy sagaState)
        {
            _mongoCollection.InsertOne(sagaState);
        }

        public void Save(List<SagaStateProxy> sagaStateProxies)
        {
            _mongoCollection.InsertMany(sagaStateProxies);
        }
    }
}