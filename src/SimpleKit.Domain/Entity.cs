using System;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace SimpleKit.Domain
{
    // ORM will never be Database and otherwise, so need to think about how to design entity effectively.
    public abstract class Entity<TKey> where TKey: IEquatable<TKey>
    {
        protected TKey Id { get; set; }
        protected new virtual bool Equals(object x, object y)
        {
            if (x == null && y == null)
                return true;
            if (x == null || y == null)
                return false;
            if (x.GetType().IsInstanceOfType(y) || y.GetType().IsInstanceOfType(x))
            {
                var xProperties = x.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
                var yProperties = y.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
                var propertyInfos = xProperties.Intersect(yProperties);
                foreach (var propertyInfo in propertyInfos)
                {
                    if (propertyInfo.GetValue(x) != propertyInfo.GetValue(y))
                        return false;
                }
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            return Equals(this, obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator == (Entity<TKey> x, Entity<TKey> y)
        {
            if (x == null && y == null)
                return true;
            return x.Id.Equals(y.Id);
        }

        public static bool operator !=(Entity<TKey> x, Entity<TKey> y)
        {
            return !(x == y);
        }
    }
}