using System;
using SimpleKit.Domain.Identity;

namespace SimpleKit.Domain.Entities
{
    public abstract class Entity : IdentityBase<Guid>
    {
        protected Entity() : base(Guid.NewGuid())
        {       
        }
        protected Entity(Guid id) : base(id)
        {
        }
    }
}