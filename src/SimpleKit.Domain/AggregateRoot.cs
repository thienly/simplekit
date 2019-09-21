using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using SimpleKit.Domain.Events;

namespace SimpleKit.Domain
{
    public interface IAggregateRoot
    {
        void AddEvent(IDomainEvent @domainEvent);
        void ClearDomainEvents();
        void ApplyEvent(IDomainEvent @domainEvent);
        IReadOnlyCollection<IDomainEvent> GetUncommittedEvents();
    }
    public class AggregateRootBase : AggregateRootWithId<Guid>
    {
        public AggregateRootBase(Guid id) : base(id)
        {
        }
        public AggregateRootBase() : this(Guid.NewGuid())
        {            
        }
    }

    public class AggregateRootWithId<TId> : EntityWithId<TId>, IAggregateRoot
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

        public IReadOnlyCollection<IDomainEvent> GetUncommittedEvents()
        {
            return _uncommitedDomainEvents.ToList();
        }

        public int Version { get; set; }
    }
}