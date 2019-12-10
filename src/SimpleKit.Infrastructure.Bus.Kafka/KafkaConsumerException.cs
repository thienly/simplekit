using System;

namespace SimpleKit.Infrastructure.Bus.Kafka
{
    public class KafkaConsumerException : Exception
    {
        public KafkaConsumerException() : base()
        {
            
        }

        public KafkaConsumerException(string message) : base(message)
        {
            
        }

        public KafkaConsumerException(string message, Exception innerException) : base(message, innerException)
        {
            
        }
    }
}