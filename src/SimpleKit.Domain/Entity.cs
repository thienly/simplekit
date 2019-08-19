using System;

namespace SimpleKit.Domain
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

    public abstract class EntityWithId<TId> : IdentityBase<TId>, IAuditable
    {
        protected EntityWithId()
        {
            
        }
        protected EntityWithId(TId id) : base(id)
        {
        }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int UpdatedBy { get; set; }
    }
}