using System;

namespace ProductMgt.ApplicationService
{ 
    public class AppServiceException : Exception
    {
        public AppServiceException()
        {
            
        }

        public AppServiceException(string message) : base(message)
        {
            
        }

        public AppServiceException(string message, Exception innerException) : base(message, innerException)
        {
            
        }
    }
}