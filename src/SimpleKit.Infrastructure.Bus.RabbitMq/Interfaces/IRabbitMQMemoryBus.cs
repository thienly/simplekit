using System.Threading.Tasks;
using SimpleKit.Domain.Events;

namespace SimpleKit.Infrastructure.Bus.RabbitMq.Interfaces
{
    public interface IRabbitMQMemoryBus
    {
        void Publish<T>(T data) where T : IntegrationEvent;
        Task Consume();
    }
}