using System;

namespace SimpleKit.Infrastructure.Repository.EfCore.Repository
{
    public class SimpleKitTransactionException : Exception
    {
        public SimpleKitTransactionException()
        {
            
        }

        public SimpleKitTransactionException(string message) : base(message)
        {
            
        }

        public SimpleKitTransactionException(string message, Exception innerException) : base(message, innerException)
        {
            
        }
    }
}