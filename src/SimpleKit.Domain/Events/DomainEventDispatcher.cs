using System.Reflection;
using System.Threading.Tasks;

namespace SimpleKit.Domain.Events
{
    public class DomainEventDispatcher : IDomainEventDispatcher
    {
        private static EventHandlerFactory _factory;

        public DomainEventDispatcher(EventHandlerFactory factory)
        {
            _factory = factory;
        }

        public void Dispatch(IDomainEvent domainEvent)
        {
            var genericHandlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
            var handlers = _factory(genericHandlerType);
            if (handlers != null)
            {
                foreach (var handler in handlers)
                {
                    var methodInfo = handler.GetType()
                        .GetMethod("Handle", BindingFlags.Public | BindingFlags.Instance);
                    var invoke = (Task) methodInfo.Invoke(handler, new[] {domainEvent});
                    invoke.GetAwaiter().GetResult();
                }
            }
        }
    }
}