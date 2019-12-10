using RabbitMQ.Client;

namespace SimpleKit.Infrastructure.Bus.RabbitMq.Interfaces
{
    public interface IRabbitMQChannelFactory
    {
        IModel CreateChannel();
    }
}