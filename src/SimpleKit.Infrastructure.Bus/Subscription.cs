using System;

namespace SimpleKit.Infrastructure.Bus
{
    public class Subscription : IEquatable<Subscription>
    {
        internal Subscription()
        {
        }
        public string EventName { get; set; }
        public Type EventHandler { get; set; }
        public bool IsDynamic { get; set; }
        public bool Equals(Subscription other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return EventName == other.EventName && EventHandler == other.EventHandler;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Subscription) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((EventName != null ? EventName.GetHashCode() : 0) * 397) ^ (EventHandler != null ? EventHandler.GetHashCode() : 0);
            }
        }

        public static Subscription Create(string eventName, Type eventType)
        {
            return new Subscription()
            {
                EventName = eventName,
                EventHandler = eventType
            };
        }
    }
}