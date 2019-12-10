using System;

namespace SimpleKit.Infrastructure.Bus
{
    public class SubscriptionManagerException : Exception
    {
        public SubscriptionManagerException()
        {
            
        }

        public SubscriptionManagerException(string message) : base(message)
        {
            
        }

        public SubscriptionManagerException(string message, Exception innerException) : base(message, innerException)
        {
            
        }
    }
}