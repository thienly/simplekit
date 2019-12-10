using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SimpleKit.Infrastructure.Bus;
using SimpleKit.Infrastructure.Bus.RabbitMq;
using SimpleKit.Infrastructure.Bus.RabbitMq.Interfaces;
using Xunit;

namespace Test.SimpleKit.Infrastructure.Bus.RabbitMq
{
    public class Test_SubscriptionManager : RabbitMqBase
    {
        [Fact]
        public void Test_add_subscription_and_get_event_name_correctly()
        {
            var subscription = _serviceProvider.GetService<ISubscriptionManager>();
            subscription.AddSubscription<UTIntegrationEvent,UTIntegrationEventHandler>();
            subscription.Should().Match<SubscriptionManager>(s => s.HasEvent(typeof(UTIntegrationEvent).FullName));
        }

        [Fact]
        public void Test_add_subscription_and_get_event_type_correctly()
        {
            var subscription = _serviceProvider.GetService<ISubscriptionManager>();
            subscription.AddSubscription<UTIntegrationEvent, UTIntegrationEventHandler>();
            subscription.Should().Match<SubscriptionManager>(s =>
                s.Subscriptions.First(x => x.EventName == typeof(UTIntegrationEvent).FullName).EventHandler != null);
        }

        [Fact]
        public void Test_remove_event_from_subsription_manager()
        {
            var subscription = _serviceProvider.GetService<ISubscriptionManager>();
            subscription.AddSubscription<UTIntegrationEvent, UTIntegrationEventHandler>();
            Assert.Throws<SubscriptionManagerException>(() => subscription.AddSubscription<UTIntegrationEvent, UTIntegrationEventHandler>());
        }
    }
}