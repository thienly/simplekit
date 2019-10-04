using System;
using SimpleKit.Domain.Identity;

namespace SimpleKit.Domain.Entities
{
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