using System;

namespace SimpleKit.Domain.Identity
{
    public interface IIdentity
    {
        Guid Id { get; set; }
    }
}