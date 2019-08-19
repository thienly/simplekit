using System;

namespace SimpleKit.Domain.Events
{
    public interface IEvent
    {
        Guid Id { get; set; }
    }
}