using System;
using System.Data;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SimpleKit.Domain.Events;
using SimpleKit.Infrastructure.Bus;
using SimpleKit.Infrastructure.Bus.RabbitMq;
using SimpleKit.Infrastructure.Bus.RabbitMq.Interfaces;

namespace Test.DatabaGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddRabbitMq(new RabbitMQOptions()
            {
                Server = "10.0.19.103",
                UserName = "guest",
                Password = "guest",
                QueueName = "simplekitqueue"
            }, Assembly.GetExecutingAssembly());
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var subscriptionManager = serviceProvider.GetService<ISubscriptionManager>();
            subscriptionManager.AddSubscription<UTIntegrationEvent,UTIntegrationEventHandler>();
            var bus = serviceProvider.GetService<IRabbitMQMemoryBus>();
            bus.Consume();
            Console.ReadLine();
        }
    }
    internal class UTIntegrationEvent : IntegrationEvent
    {
            
    }
    internal class UTIntegrationEventHandler : IIntegrationEventHandler<UTIntegrationEvent>
    {
        public Task Handle(UTIntegrationEvent data)
        {
            return Task.CompletedTask;
        }
    }
}