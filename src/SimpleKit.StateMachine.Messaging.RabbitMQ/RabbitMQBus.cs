using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Framing;
using SimpleKit.StateMachine.Definitions;

namespace SimpleKit.StateMachine.Messaging.RabbitMQ
{
    public class RabbitMqSagaPublisher : ISagaPublisher
    {
        private RabbitMQBus _rabbitMqBus;

        public RabbitMqSagaPublisher(RabbitMQBus rabbitMqBus)
        {
            _rabbitMqBus = rabbitMqBus;
        }

        public void Publish(SagaCommandContext context, byte[] data)
        {
            _rabbitMqBus.Publish(context, data);
        }
    }
    public class RabbitMQBus
    {
        private IRabbitMQChannelFactory _channelFactory;
        public RabbitMQBus(IRabbitMQChannelFactory channelFactory)
        {
            _channelFactory = channelFactory;
        }
        public void Publish(SagaCommandContext context, byte[] data)
        {
            using (var channel = _channelFactory.CreateChannel())
            {
                channel.ExchangeDeclare(context.MessageType, "direct", true, false, null);
                channel.QueueDeclare(context.DestinationAddress, true, false, false, null);
                channel.QueueDeclare(context.ReplyAddress, true, false, false, null);
                channel.QueueDeclare(context.FaultAddress, true, false, false, null);
                channel.QueueBind(context.DestinationAddress, context.MessageType, "", null);
                channel.BasicPublish(context.MessageType, "", true, new BasicProperties()
                {
                    Headers = context.ToDictionary(),
                    Persistent = true,
                }, data);
                // we start to consume message on reply queue.
                StartConsumingMessage(context);
            }
        }

        public void StartConsumingMessage(SagaCommandContext context) 
        {
            using (var channel = _channelFactory.CreateChannel())
            {
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (sender, args) =>
                {
                        
                };
                channel.BasicConsume(context.ReplyAddress, false, context.Host, consumer);
            }
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSagaRabbitMq(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddSingleton<IRabbitMQChannelFactory, RabbitMqChannelFactory>();
            serviceCollection.AddSingleton<RabbitMQBus>();
            serviceCollection.Configure<RabbitMQOptions>(configuration.GetSection("RabbitMQ"));
            serviceCollection.AddSingleton<ISagaPublisher, RabbitMqSagaPublisher>();
            return serviceCollection;
        }
    }
}