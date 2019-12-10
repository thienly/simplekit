using System;

namespace SimpleKit.Infrastructure.Bus.Kafka
{
    [AttributeUsage(AttributeTargets.Class)]
    public class KafkaTopicAttribute : Attribute
    {
        public string[] TopicName { get; }
        public KafkaTopicAttribute(params string [] topicNames)
        {
            TopicName = topicNames;
        }
    }
}