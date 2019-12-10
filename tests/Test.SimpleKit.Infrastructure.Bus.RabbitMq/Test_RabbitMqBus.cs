using System;
using System.Buffers.Text;
using System.Threading;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SimpleKit.Infrastructure.Bus;
using SimpleKit.Infrastructure.Bus.RabbitMq.Interfaces;
using Xunit;

namespace Test.SimpleKit.Infrastructure.Bus.RabbitMq
{
    public class Test_RabbitMqBus : RabbitMqBase
    {
        [Fact]
        public void Test_if_can_publish_a_message_to_rabbit()
        {
            var subscriptionManager = _serviceProvider.GetService<ISubscriptionManager>();
            subscriptionManager.AddSubscription<UTIntegrationEvent, UTIntegrationEventHandler>();
            var bus = _serviceProvider.GetService<IRabbitMQMemoryBus>();
            bus.Publish(new UTIntegrationEvent());
        }


        [Fact]
        public void Test_if_can_consume_a_message_from_rabbit_one_handler()
        {
            var subscriptionManager = _serviceProvider.GetService<ISubscriptionManager>();
            subscriptionManager.AddSubscription<UTIntegrationEvent, UTIntegrationEventHandler>();
            var bus = _serviceProvider.GetService<IRabbitMQMemoryBus>();
            bus.Consume();
        }
    }
}