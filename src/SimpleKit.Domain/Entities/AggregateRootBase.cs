using System;

namespace SimpleKit.Domain.Entities
{
    public class AggregateRootBase : AggregateRootWithId<Guid>
    {
        public AggregateRootBase(Guid id) : base(id)
        {
        }
        public AggregateRootBase() : this(Guid.NewGuid())
        {            
        }
    }
}