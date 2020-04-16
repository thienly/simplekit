using RabbitMQ.Client;

namespace SimpleKit.StateMachine.Messaging.RabbitMQ
{
    public interface IRabbitMQChannelFactory
    {
        IModel CreateChannel();
    }
}