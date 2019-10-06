using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace SimpleKit.Domain.Events
{
    public class DomainEvents
    {
        private static Func<Type, IEnumerable<object>> _factory;

        public DomainEvents(Func<Type, IEnumerable<object>> factory)
        {
            _factory = factory;
        }
        public static void Raise(IDomainEvent domainEvent)
        {
            var genericHandlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
            var handlers = _factory(genericHandlerType);
            foreach (var handler in handlers)
            {
                var methodInfo = handler.GetType().GetMethod("Handle",BindingFlags.Public|BindingFlags.Instance);
                var invoke = (Task)methodInfo.Invoke(handler, new[] {domainEvent});
                invoke.GetAwaiter().GetResult();
            }
        }
    }
}