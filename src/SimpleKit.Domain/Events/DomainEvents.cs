using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace SimpleKit.Domain.Events
{
    public delegate IEnumerable<object> EventHandlerFactory(Type type);
    public class DomainEvents
    {
        private static EventHandlerFactory _factory;

        public DomainEvents(EventHandlerFactory factory = null)
        {
            _factory = factory;
        }
        public static void Raise(IDomainEvent domainEvent)
        {
            if (_factory != null)
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
}