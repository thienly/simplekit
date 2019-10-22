using System;
using System.Linq;
using System.Reflection;

namespace SimpleKit.Domain
{
    public abstract class ValueObject<T> where T: ValueObject<T>
    {
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

        public static bool operator == (ValueObject<T> x, ValueObject<T> y)
        {
            if (ReferenceEquals(null, x))
                return false;
            return  x.Equals(y);
        }

        public static bool operator !=(ValueObject<T> x, ValueObject<T> y)
        {
            return !(x == y);
        }
    }
}