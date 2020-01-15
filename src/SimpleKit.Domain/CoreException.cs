using System;

namespace SimpleKit.Domain
{
    public class CoreException : Exception
    {
        public CoreException()
        {
            
        }

        public CoreException(string message) : base(message)
        {
            
        }

        public CoreException(string message, Exception innerException) : base(message, innerException)
        {
            
        }
    }
}