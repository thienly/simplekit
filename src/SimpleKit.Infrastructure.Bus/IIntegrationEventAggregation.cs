using System;
using System.Threading.Tasks;

namespace SimpleKit.Infrastructure.Bus
{
    public delegate object IntegrationEventHandlerFactory(Type type);
    public interface IIntegrationEventAggregation
    {
        Task Process(string body, string eventName);
    }
}