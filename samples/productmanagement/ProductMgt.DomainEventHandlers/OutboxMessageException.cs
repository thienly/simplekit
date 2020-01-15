using System;

namespace ProductMgt.DomainEventHandlers
{
    public class OutboxMessageException : Exception
    {
        public OutboxMessageException()
        {
            
        }

        public OutboxMessageException(string message) : base(message)
        {
            
        }

        public OutboxMessageException(string message, Exception innerException) : base(message, innerException)
        {
        }
        
    }
}