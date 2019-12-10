using System.Collections.Generic;
using SimpleKit.Domain.Events;

namespace SimpleKit.Infrastructure.Bus
{
    public interface ISubscriptionManager
    {
        void AddDynamicSubscription<TH>(string eventName) where TH : IDynamicIntegrationEventHandler;
        void AddSubscription<T, TH>() where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;
        IReadOnlyCollection<Subscription> Subscriptions { get; }
        bool HasEvent(string eventName);
        IReadOnlyCollection<string> GetAllEvents();
        void RemoveSubscription(string eventName);
    }
    
}