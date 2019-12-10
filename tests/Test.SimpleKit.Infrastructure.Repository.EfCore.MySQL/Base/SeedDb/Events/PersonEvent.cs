using System;
using SimpleKit.Domain.Events;

namespace Test.SimpleKit.Infrastructure.Repository.EfCore.MySQL.Base.SeedDb.Events
{
    public abstract class PersonEvent: IDomainEvent
    {
        public Person Person { get; set; }
        public Guid EventId { get; set; }
    }
}