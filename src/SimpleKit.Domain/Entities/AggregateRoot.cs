using System;
using System.Collections.Generic;
using System.Linq;
using SimpleKit.Domain.Events;

namespace SimpleKit.Domain.Entities
{
    public class AggregateRootWithId<TId> : EntityWithId<TId>, IAggregateRoot
    {
        private ICollection<IDomainEvent> _uncommitedDomainEvents = new List<IDomainEvent>();
        protected AggregateRootWithId(TId id) : base(id)
        {
        }

        protected AggregateRootWithId()
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

        public IReadOnlyCollection<IDomainEvent> GetUncommittedEvents()
        {
            return _uncommitedDomainEvents.ToList();
        }
        public byte[] Version { get; set; }
    }
}