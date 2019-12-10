using System.Threading.Tasks;
using AutoFixture;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SimpleKit.Infrastructure.Bus;
using Xunit;

namespace Test.SimpleKit.Infrastructure.Bus.RabbitMq
{
    public class Test_IntegrationEventAggregation : RabbitMqBase
    {
        [Fact]
        public async Task Test_if_can_process_data_with_a_integration_handler()
        {
            var subscriptionManager = _serviceProvider.GetService<ISubscriptionManager>();
            subscriptionManager.AddSubscription<UTIntegrationEvent, UTIntegrationEventHandler>();
            subscriptionManager.AddSubscription<UTIntegrationEvent, UTIntegrationEventHandlerSecond>();
            var utIntegrationEvent = _fixture.Create<UTIntegrationEvent>();
            var integrationEventAggregation = _serviceProvider.GetService<IIntegrationEventAggregation>();
            await integrationEventAggregation.Process(JsonConvert.SerializeObject(utIntegrationEvent), typeof(UTIntegrationEvent).FullName);
        }
        [Fact]
        public async Task Test_if_can_process_data_dynamic_integration_handler()
        {
            var subscriptionManager = _serviceProvider.GetService<ISubscriptionManager>();
            subscriptionManager.AddDynamicSubscription<DynamicIntegrationEventHandler>(typeof(UTIntegrationEvent).FullName);
            subscriptionManager.AddSubscription<UTIntegrationEvent, UTIntegrationEventHandler>();
            subscriptionManager.AddSubscription<UTIntegrationEvent, UTIntegrationEventHandlerSecond>();
            var utIntegrationEvent = _fixture.Create<UTIntegrationEvent>();
            var integrationEventAggregation = _serviceProvider.GetService<IIntegrationEventAggregation>();
            await integrationEventAggregation.Process(JsonConvert.SerializeObject(utIntegrationEvent), typeof(UTIntegrationEvent).FullName);
        }
    }
}