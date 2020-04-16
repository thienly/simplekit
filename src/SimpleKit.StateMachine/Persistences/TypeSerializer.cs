using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace SimpleKit.StateMachine.Persistences
{
    public class TypeSerializer : IBsonSerializer
    {
        public object Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            if (context.Reader.CurrentBsonType == BsonType.Null)
            {
                return null;
            }
            else
            {
                var fullName = context.Reader.ReadString();
                return Type.GetType(fullName);
            }
        }

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        {
            if (value == null)
            {
                context.Writer.WriteNull();
            }
            else
            {
                context.Writer.WriteString(((Type) value).AssemblyQualifiedName);
            }
        }

        public Type ValueType { get; } = typeof(Type);
    }
}