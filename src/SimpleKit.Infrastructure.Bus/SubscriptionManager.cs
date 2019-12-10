using System.Collections.Generic;
using System.Linq;
using SimpleKit.Domain.Events;

namespace SimpleKit.Infrastructure.Bus
{
    public class SubscriptionManager :  ISubscriptionManager
    {
        private List<Subscription> _subscriptions = new List<Subscription>();
        public void AddDynamicSubscription<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
        {
            if (_subscriptions.Any(x=>x.EventName == eventName && x.EventHandler == typeof(TH)))
                throw new SubscriptionManagerException($"There is already handler {typeof(TH)} for event {eventName}");
            _subscriptions.Add(new Subscription()
            {
                EventName = eventName,
                EventHandler = typeof(TH),
                IsDynamic = true
            });
        }

        public void AddSubscription<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>
        {
            if (_subscriptions.Any(x=>x.EventName == typeof(T).FullName && x.EventHandler == typeof(TH)))
                throw new SubscriptionManagerException($"There is already handler {typeof(TH)} for event {typeof(T)}");
            _subscriptions.Add(new Subscription()
            {
                EventName = typeof(T).FullName,
                EventHandler = typeof(TH),
                IsDynamic = false
            });
        }
        public IReadOnlyCollection<Subscription> Subscriptions => _subscriptions;
        public bool HasEvent(string eventName)
        {
            return _subscriptions.Any(x => x.EventName == eventName);
        }


        public IReadOnlyCollection<string> GetAllEvents()
        {
            return _subscriptions.Select(x => x.EventName).ToList();
        }
        public void RemoveSubscription(string eventName)
        {
            if (!HasEvent(eventName))
            {
                throw new SubscriptionManagerException("The subscription is not existed");
            }
            _subscriptions.RemoveAll(s => s.EventName == eventName);
        }
    }
}