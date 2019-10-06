using System;
using SimpleKit.Domain.Events;

namespace SampleApiWithEfCore.Models.Events
{
    public abstract class PersonEvent: IDomainEvent
    {
        public Person Person { get; set; }
        public Guid EventId { get; set; }
    }
}