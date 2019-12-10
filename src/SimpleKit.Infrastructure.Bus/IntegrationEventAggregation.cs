using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SimpleKit.Infrastructure.Bus
{
    public class IntegrationEventAggregation : IIntegrationEventAggregation
    {
        private IntegrationEventHandlerFactory _eventHandlerFactory;
        private ISubscriptionManager _subscriptionManager;

        public IntegrationEventAggregation(IntegrationEventHandlerFactory eventHandlerFactory,
            ISubscriptionManager subscriptionManager)
        {
            _eventHandlerFactory = eventHandlerFactory;
            _subscriptionManager = subscriptionManager;
        }

        public async Task Process(string body, string eventName)
        {
            var subHandlers = _subscriptionManager.Subscriptions.Where(x => x.EventName == eventName);
            foreach (var subHandler in subHandlers)
            {
                if (typeof(IDynamicIntegrationEventHandler).IsAssignableFrom(subHandler.EventHandler))
                {
                    var handler = (IDynamicIntegrationEventHandler) _eventHandlerFactory(subHandler.EventHandler);
                    var task = (Task) handler.Handle(body);
                    await task;
                }
                else
                {
                    var interfaces = subHandler.EventHandler.GetInterfaces();
                    var interf = interfaces.First(x =>
                        x.GetGenericTypeDefinition() == typeof(IIntegrationEventHandler<>));
                    var typeEvent = interf.GetGenericArguments()[0];
                    var handler = _eventHandlerFactory(subHandler.EventHandler);
                    var methodInfo =
                        subHandler.EventHandler.GetMethod("Handle", BindingFlags.Instance | BindingFlags.Public);
                    var data = JsonConvert.DeserializeObject(body, typeEvent);
                    var task = (Task) methodInfo.Invoke(handler, new[] {data});
                    await task;
                }
            }
        }
    }
}