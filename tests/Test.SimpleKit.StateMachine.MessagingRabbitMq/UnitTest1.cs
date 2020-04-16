using System;
using AutoFixture;
using SimpleKit.StateMachine.Definitions;
using SimpleKit.StateMachine.Messaging.RabbitMQ;
using Xunit;

namespace Test.SimpleKit.StateMachine.MessagingRabbitMq
{
    public class UnitTest1
    {
        [Fact]
        public void Test_if_can_publish_message_to_correct_queue()
        {
            var factory = new RabbitMqChannelFactory(new RabbitMQOptions()
            {
                Server = "45.118.148.55",
                UserName = "guest",
                Password = "guest"
            });
            var bus = new RabbitMQBus(factory);
            var fixture = new Fixture();
            var context = fixture.Create<SagaCommandContext>();
            bus.Publish(context, new SampleMessage()
            {
                DateTime = DateTime.Now
            });
        }

        internal class SampleMessage
        {
            public DateTime DateTime { get; set; }
        }
    }
}