using System;
using System.Threading.Tasks;
using SimpleKit.Domain.Events;

namespace SimpleKit.Infrastructure.Bus
{
    public interface IIntegrationEventHandler<T>
    {
        Task Handle(T data);
    }
    public interface IDynamicIntegrationEventHandler
    {
        Task Handle(dynamic data);
    }
}