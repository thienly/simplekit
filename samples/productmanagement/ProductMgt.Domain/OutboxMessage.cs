using System;
using SimpleKit.Domain.Entities;

namespace ProductMgt.Domain
{
    public class OutboxMessage : AggregateRootBase
    {
        public Type Type { get; set; }
        public string Body { get; set; }
        public long DispatchedTime { get; set; }
        public long ProcessedTime { get; set; }
        public int NumberOfRetries { get; set; }
    }
}