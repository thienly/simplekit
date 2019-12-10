using System.Diagnostics.CodeAnalysis;
using Castle.DynamicProxy;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SimpleKit.Infrastructure.Bus.RabbitMq.Interfaces;
using Xunit;

namespace Test.SimpleKit.Infrastructure.Bus.RabbitMq
{
    public class ConnectionTesting : RabbitMqBase
    {
        [Fact]
        public void Test_if_connection_can_connect()
        {
            var rabbitMqConnectionFactory = _serviceProvider.GetService<IRabbitMQChannelFactory>();
            var connection = rabbitMqConnectionFactory.CreateChannel();
            connection.IsOpen.Should().BeTrue();
        }
    }
}