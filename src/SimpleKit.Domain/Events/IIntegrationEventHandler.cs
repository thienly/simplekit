using System.Threading.Tasks;

namespace SimpleKit.Domain.Events
{
    public interface IIntegrationEventHandler<in TIntegrationEvent> 
                                            where TIntegrationEvent : IIntegrationEvent
    {
        Task Handle(TIntegrationEvent @event);
    }
}