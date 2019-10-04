using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace SimpleKit.Domain.Events
{
    public class DomainEvents
    {
        public static Func<Type, IEnumerable<object>> _factory;

        public static void Raise(IDomainEvent domainEvent)
        {
            var handlers = _factory(domainEvent.GetType());
            foreach (var handler in handlers)
            {
                var methodInfo = handler.GetType().GetMethod("Handle",BindingFlags.Public|BindingFlags.Instance);
                Task.Run(() => methodInfo.Invoke(handler, new[] {domainEvent})).GetAwaiter().GetResult();

            }
        }
    }
}