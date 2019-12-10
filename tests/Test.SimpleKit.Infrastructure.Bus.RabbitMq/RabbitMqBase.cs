using System;
using System.Reflection;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.Extensions.DependencyInjection;
using SimpleKit.Domain.Events;
using SimpleKit.Infrastructure.Bus;
using SimpleKit.Infrastructure.Bus.RabbitMq;

namespace Test.SimpleKit.Infrastructure.Bus.RabbitMq
{
    public abstract class RabbitMqBase
    {
        protected IFixture _fixture;
        protected IServiceProvider _serviceProvider;
        public RabbitMqBase()
        {
            _fixture = new Fixture();
            _fixture.Customize(new AutoMoqCustomization());
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddRabbitMq(new RabbitMQOptions()
            {
                Server = "10.0.19.103",
                UserName = "guest",
                Password = "guest",
                QueueName = "simplekitqueue"
            }, Assembly.GetExecutingAssembly());
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }
        public class UTIntegrationEvent : IntegrationEvent
        {
            
        }
        public class DynamicIntegrationEventHandler : IDynamicIntegrationEventHandler
        {
            public Task Handle(dynamic data)
            {
                return Task.CompletedTask;
            }
        }
        public class UTIntegrationEventHandler : IIntegrationEventHandler<UTIntegrationEvent>
        {
            public Task Handle(UTIntegrationEvent data)
            {
                return Task.CompletedTask;
            }
        }
        public class UTIntegrationEventHandlerSecond : IIntegrationEventHandler<UTIntegrationEvent>
        {
            public Task Handle(UTIntegrationEvent data)
            {
                return Task.CompletedTask;
            }
        }
    }
}