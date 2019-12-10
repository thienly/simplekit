using System;
using System.Reflection;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SimpleKit.Domain.Events;
using SimpleKit.Infrastructure.Bus;
using SimpleKit.Infrastructure.Bus.Kafka;
using Test.SimpleKit.Base;
using Xunit;
using Xunit.Abstractions;

namespace Test.SimpleKit.Infrastructure.Bus.Kafka
{
    public abstract class KafkaBusBase
    {
        protected IFixture _fixture;
        protected IServiceProvider _serviceProvider;
        private ITestOutputHelper _testOutputHelper;
        public KafkaBusBase(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _fixture = new Fixture();
            _fixture.Customize(new AutoMoqCustomization());
            var serviceCollection = new ServiceCollection();
            var loF = new UnitTestLoggerFactory(_testOutputHelper);
            loF.AddProvider();
            serviceCollection.AddKafkaBus(new KafkaOptions()
            {
                KafkaHost = "10.0.19.103:9092",
                GroupId = "unitest-group"
            }, loF,Assembly.GetExecutingAssembly());
            _serviceProvider = serviceCollection.BuildServiceProvider();
            var subscriptionManager = _serviceProvider.GetService<ISubscriptionManager>();
            subscriptionManager.AddSubscription<UTIntegrationEvent,UTIntegrationEventHandler>();
            subscriptionManager.AddSubscription<UTIntegrationEvent,UTIntegrationEventHandlerSecond>();
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