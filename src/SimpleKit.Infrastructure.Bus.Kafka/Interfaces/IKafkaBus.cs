using System.Threading.Tasks;
using SimpleKit.Domain.Events;

namespace SimpleKit.Infrastructure.Bus.Kafka.Interfaces
{
    public interface IKafkaBus
    {
        void Consume();
        Task Publish<T>(T data) where T : IntegrationEvent;
    }
}