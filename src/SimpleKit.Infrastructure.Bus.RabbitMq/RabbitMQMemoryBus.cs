using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SimpleKit.Domain.Events;
using SimpleKit.Infrastructure.Bus.RabbitMq.Interfaces;

namespace SimpleKit.Infrastructure.Bus.RabbitMq
{
    public class RabbitMQMemoryBus : IRabbitMQMemoryBus
    {
        private IRabbitMQChannelFactory _channelFactory;
        private ISubscriptionManager _subscriptionManager;
        private readonly IIntegrationEventAggregation _integrationEventAggregation;
        private string _queueName;
        
        private ILogger<RabbitMqChannelFactory> Logger { get; set; }

        public RabbitMQMemoryBus(IRabbitMQChannelFactory channelFactory, RabbitMQOptions options,
            ISubscriptionManager subscriptionManager, IIntegrationEventAggregation integrationEventAggregation)
        {
            _channelFactory = channelFactory;
            _subscriptionManager = subscriptionManager;
            _integrationEventAggregation = integrationEventAggregation;
            _queueName = options.QueueName;
            Logger = NullLogger<RabbitMqChannelFactory>.Instance;
            InitializeQueue();
        }

        public void Publish<T>(T data) where T : IntegrationEvent
        {
            Logger.LogInformation("Begin publishing data");
            using (var channel = _channelFactory.CreateChannel())
            {
                var @eventName = typeof(T).FullName;
                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
                channel.BasicPublish(@eventName, "", null, body);
                Logger.LogInformation("End publishing");
            }
        }

        private void InitializeQueue()
        {
            using (var channel = _channelFactory.CreateChannel())
            {
                var queueDeclareOk = channel.QueueDeclare(_queueName, true, false, false);
                foreach (var allEvent in _subscriptionManager.GetAllEvents())
                {
                    channel.ExchangeDeclare(allEvent, "fanout", true, false);
                    channel.QueueBind(queueDeclareOk.QueueName, allEvent, "");
                }
            }
        }

        public Task Consume()
        {
            var channel = _channelFactory.CreateChannel();
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (sender, args) =>
            {
                var eventName = args.Exchange;
                var body = Encoding.UTF8.GetString(args.Body);
                await _integrationEventAggregation.Process(body, eventName);
                channel.BasicAck(args.DeliveryTag, false);
            };
            channel.BasicConsume(_queueName, false, consumer);
            channel.CallbackException += (sender, args) =>
            {
                channel.Dispose();
                Consume();
            };
            return Task.CompletedTask;
        }
    }
}