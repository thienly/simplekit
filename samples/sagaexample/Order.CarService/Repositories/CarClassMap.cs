using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.IdGenerators;
using Order.CarService.Domains;
using SimpleKit.Domain.Entities;
using SimpleKit.Domain.Identity;

namespace Order.CarService.Repositories
{
    public static class CarClassMap
    {
        public static void Customize()
        {
            var pack = new ConventionPack
            {
                new EnumRepresentationConvention(BsonType.String)
            };
            ConventionRegistry.Register("EnumAsString", pack, t => true);
            BsonClassMap.RegisterClassMap<AggregateRootBase>();
            BsonClassMap.RegisterClassMap<AggregateRootWithId<Guid>>();
            BsonClassMap.RegisterClassMap<EntityWithId<Guid>>();
            BsonClassMap.RegisterClassMap<IdentityBase<Guid>>(cm =>
            {
                cm.SetIsRootClass(true);
                cm.MapIdProperty(x => x.Id).SetIdGenerator(GuidGenerator.Instance);
            });
            BsonClassMap.RegisterClassMap<Car>(cm =>
            {
                cm.AutoMap();
            });
        }
    }
}