using System.Collections.Generic;
using SimpleKit.Domain.Events;

namespace SimpleKit.Domain.Entities
{
    public interface IAggregateRoot
    {
        void AddEvent(IDomainEvent @domainEvent);
        void ClearDomainEvents();
        IReadOnlyCollection<IDomainEvent> GetUncommittedEvents();
        byte[] Version { get; set; }
    }
}