using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.Extensions.DependencyInjection;
using SimpleKit.Infrastructure.Bus.Kafka.Interfaces;
using Xunit;
using Xunit.Abstractions;

namespace Test.SimpleKit.Infrastructure.Bus.Kafka
{
    public class Test_KafkaBus : KafkaBusBase
    {
        public Test_KafkaBus(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public async Task Test_If_can_publish_to_topic()
        {
            var kafkaBus = _serviceProvider.GetService<IKafkaBus>();
            var @events = _fixture.CreateMany<UTIntegrationEvent>(100);
            var lst = new List<Task>();
            foreach (var @event in events)
            {
                 lst.Add(kafkaBus.Publish(@event));    
            }

            await Task.WhenAll(lst);
        }

        [Fact]
        public async Task Test_if_can_consume_message_from_topic()
        {
            var kafkaBus = _serviceProvider.GetService<IKafkaBus>();
            kafkaBus.Consume();
        }
    }
}