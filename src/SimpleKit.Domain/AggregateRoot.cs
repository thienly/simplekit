using System;
using System.Collections.Generic;
using SimpleKit.Domain.Events;

namespace SimpleKit.Domain
{
    public class AggregateRootBase : AggregateRootWithId<Guid>
    {
        public AggregateRootBase(Guid id) : base(id)
        {
        }
        public AggregateRootBase() : this(Guid.NewGuid())
        {            
        }
    }

    public class AggregateRootWithId<TId> : EntityWithId<TId>
    {
        private ICollection<IDomainEvent> _uncommitedDomainEvents = new List<IDomainEvent>();
        protected AggregateRootWithId(TId id) : base(id)
        {
        }

        public void SetId(TId id)
        {
            Id = id;
        }

        public void AddEvent(IDomainEvent @domainEvent)
        {
            _uncommitedDomainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _uncommitedDomainEvents.Clear();
        }
        public void ApplyEvent(IDomainEvent @domainEvent)
        {            
            DomainEvents.Raise(domainEvent);
        }

        public int Version { get; set; }
    }
}