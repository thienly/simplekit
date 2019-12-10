using System;
using SimpleKit.Domain.Events;

namespace Test.SimpleKit.Infrastructure.Repository.MongoDb.SeedDb.Events
{
    public abstract class PersonEvent: IDomainEvent
    {
        public Person Person { get; set; }
        public Guid EventId { get; set; }
    }
}