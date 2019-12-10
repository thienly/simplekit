using System;

namespace SimpleKit.Domain.Events
{
    public abstract class IntegrationEvent
    {
        public Guid MessageId { get; set; } = Guid.NewGuid();
        public Guid CorrelationId { get; set; } = Guid.NewGuid();
        public long CreatedTime { get; set; } = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
        public long ProcessedTime { get; set; } = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
    }
}