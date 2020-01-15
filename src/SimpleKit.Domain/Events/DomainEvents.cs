using System;
using System.Collections.Generic;

namespace SimpleKit.Domain.Events
{
    public delegate IEnumerable<object> EventHandlerFactory(Type type);
}