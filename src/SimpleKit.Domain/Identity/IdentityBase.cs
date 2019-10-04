using System;

namespace SimpleKit.Domain.Identity
{
    public abstract class IdentityBase<TId> : IEquatable<IdentityBase<TId>>, IIdentityWithId<TId>
    {
        public IdentityBase()
        {
            
        }
        public IdentityBase(TId id)
        {
            Id = id;
        }
        public bool Equals(IdentityBase<TId> id)
        {
            if (ReferenceEquals(this, id))
                return true;
            return !ReferenceEquals(null, id) && Id.Equals(id.Id);
        }
        public TId Id { get; protected set; }

        public override bool Equals(object anotherObject)
        {
            return Equals(anotherObject as IdentityBase<TId>);
        }

        public override int GetHashCode()
        {
            return GetType().GetHashCode()+ Id.GetHashCode();
        }

        public static bool operator ==(IdentityBase<TId> x, IdentityBase<TId> y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(IdentityBase<TId> x, IdentityBase<TId> y)
        {
            return !x.Equals(y);
        }
    }

    public abstract class IdentityBase : IIdentity, IEquatable<Guid>, IIdentityWithId<Guid>
    {
        public IdentityBase(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }

        public bool Equals(Guid other)
        {
            return this.Id == other;
        }
    }
}