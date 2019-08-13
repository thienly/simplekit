using System;
using System.Collections.Generic;

namespace SimpleKit.Domain
{
    public abstract class AggregateRoot<TKey> :  Entity<TKey> where TKey: IEquatable<TKey>
    {
        private List<IDomainEvent> _domainEvents = new List<IDomainEvent>();
        public List<IDomainEvent> UncommitedEvents()
        {
            return _domainEvents;
        }
        protected void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }
        public void ClearAllEvents()
        {
            _domainEvents.Clear();
        }        
    }
}