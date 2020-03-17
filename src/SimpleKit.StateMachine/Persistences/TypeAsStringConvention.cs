using System;
using System.Reflection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

namespace SimpleKit.StateMachine.Persistences
{
    public class TypeAsStringConvention : ConventionBase, IMemberMapConvention
    {
        private readonly BsonType _representation;

        public TypeAsStringConvention(BsonType representation)
        {
            _representation = representation;
        }

        public string Name { get; }

        public void Apply(BsonMemberMap memberMap)
        {
            Type memberType = memberMap.MemberType;
            if (memberType.GetTypeInfo() == typeof(Type) && memberMap.MemberName == "SagaDefinitionType")
            {
                if (!(memberMap.GetSerializer() is IRepresentationConfigurable serializer))
                    return;
                IBsonSerializer serializer1 = serializer.WithRepresentation(this._representation);
                memberMap.SetSerializer(serializer1);
            }
        }
    }
}