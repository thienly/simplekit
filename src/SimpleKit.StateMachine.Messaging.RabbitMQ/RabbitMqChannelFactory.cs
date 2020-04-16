using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace SimpleKit.StateMachine.Messaging.RabbitMQ
{
    public class RabbitMqChannelFactory : IRabbitMQChannelFactory, IDisposable
    {
        private RabbitMQOptions _rabbitMqOptions;
        private IConnection _connection;
        public ILogger Logger { get; set; }
        public RabbitMqChannelFactory(IOptions<RabbitMQOptions> rabbitMqOptions)
        {
            _rabbitMqOptions = rabbitMqOptions.Value;
            Logger = NullLogger<RabbitMqChannelFactory>.Instance;
        }
        
        public IModel CreateChannel()
        {
            var connection = CreateConnection();
            return connection.CreateModel();
        }

        private bool IsConnected => _connection != null && _connection.IsOpen;

        private IConnection CreateConnection()
        {
            if (!IsConnected)
            {
                var connectionFactory = new ConnectionFactory()
                {
                    HostName = _rabbitMqOptions.Server,
                    UserName = _rabbitMqOptions.UserName,
                    Password = _rabbitMqOptions.Password
                };
                _connection = connectionFactory.CreateConnection();
                _connection.ConnectionShutdown += ConnectionOnConnectionShutdown;
                _connection.ConnectionBlocked += ConnectionOnConnectionBlocked;
                _connection.ConnectionUnblocked += ConnectionOnConnectionUnblocked;
                _connection.ConnectionRecoveryError += ConnectionOnConnectionRecoveryError;
            }
            return _connection;
        }

        private void ConnectionOnConnectionRecoveryError(object sender, ConnectionRecoveryErrorEventArgs e)
        {
            Logger.LogError(e.Exception, "RabbitMQConnection recovery with error");
        }

        private void ConnectionOnConnectionUnblocked(object sender, EventArgs e)
        {
            Logger.LogError("Connection is unblocked");
        }
        private void ConnectionOnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        {
            Logger.LogError($"Connection is blocked with reason {e.Reason}");
        }

        private void ConnectionOnConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            Logger.LogError($"The Connection is shutdown with reason {e?.Cause}");
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}