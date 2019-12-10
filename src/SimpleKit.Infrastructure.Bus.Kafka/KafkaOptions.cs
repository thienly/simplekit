using System;

namespace SimpleKit.Infrastructure.Bus.Kafka
{
    public class KafkaOptions
    {
        public string KafkaHost { get; set; }
        public string GroupId { get; set; }
        public string DefaultTopic { get; set; }
    }
}