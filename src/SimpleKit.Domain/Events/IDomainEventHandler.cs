using System.Threading.Tasks;

namespace SimpleKit.Domain.Events
{
    public interface IDomainEventHandler{}
    public interface IDomainEventHandler<in TDomainEvent> : IDomainEventHandler
                                                            where TDomainEvent : IDomainEvent
    {
        Task Handle(TDomainEvent @event);
    }
}