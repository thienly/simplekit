using System;
using SimpleKit.Domain.Events;

namespace Test.SimpleKit.Repository.EfCore.Base.SeedDb.Events
{
    public abstract class PersonEvent: IDomainEvent
    {
        public Person Person { get; set; }
        public Guid EventId { get; set; }
    }
}