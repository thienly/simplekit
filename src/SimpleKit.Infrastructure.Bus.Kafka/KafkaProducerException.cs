using System;

namespace SimpleKit.Infrastructure.Bus.Kafka
{
    public class KafkaProducerException : Exception
    {
        public KafkaProducerException() : base()
        {
            
        }

        public KafkaProducerException(string message) : base(message)
        {
            
        }

        public KafkaProducerException(string message, Exception innerException) : base(message, innerException)
        {
            
        }
    }
}