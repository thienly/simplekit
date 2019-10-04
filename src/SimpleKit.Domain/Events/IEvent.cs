using System;

namespace SimpleKit.Domain.Events
{
    public interface IEvent
    {
        Guid EventId { get; set; }
    }
}