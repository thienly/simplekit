using System.Collections.Generic;
using SimpleKit.Domain.Events;

namespace SimpleKit.Domain.Entities
{
    public interface IAggregateRoot
    {
        void AddEvent(IDomainEvent @domainEvent);
        void ClearDomainEvents();
        void ApplyEvent(IDomainEvent @domainEvent);
        IReadOnlyCollection<IDomainEvent> GetUncommittedEvents();
        int Version { get; set; }
    }
}