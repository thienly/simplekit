using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace SimpleKit.StateMachine.Persistences
{
    internal class ServerHosting
    {
        public void Bootstrap(MongoUrl mongoUrl)
        {
            var mongoClient = new MongoClient(mongoUrl);
            var mongoDatabase = mongoClient.GetDatabase(mongoUrl.DatabaseName);
            var conventionPack = new ConventionPack();
            conventionPack.Add(new EnumRepresentationConvention(BsonType.String));

            ConventionRegistry.Register("EnumAsString", conventionPack, f => true);
                
            BsonClassMap.RegisterClassMap<SagaStateProxy>(map =>
            {
                map.SetIsRootClass(true);
                map.AutoMap();
                map.MapIdProperty(x => x.Id).SetSerializer(new GuidSerializer(BsonType.String));
                map.MapProperty(x => x.SagaId).SetSerializer(new GuidSerializer(BsonType.String));
                map.MapProperty(x => x.SagaDefinitionType).SetSerializer(new TypeSerializer());
            });

            var mongoCollection = mongoDatabase.GetCollection<SagaStateProxy>(nameof(SagaStateProxy));

            IndexKeysDefinitionBuilder<SagaStateProxy> states = new IndexKeysDefinitionBuilder<SagaStateProxy>();
            var indexKeysDefinition = states.Ascending(x => x.SagaId).Descending(x => x.Version);
            var indexModel = new CreateIndexModel<SagaStateProxy>(indexKeysDefinition, new CreateIndexOptions()
            {
                Unique = true
            });
            mongoCollection.Indexes.CreateOne(indexModel);
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSagaManager(this IServiceCollection serviceCollection, MongoUrl mongoUrl)
        {
            var serverInfo = new ServerHosting();
            serverInfo.Bootstrap(mongoUrl);
            return serviceCollection;
        }
    }
}