using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Framing;
using SagaContract;
using SagaContract.Extensions;
using SagaContract.Options;

namespace Order.HotelService.Consumers
{
    public class SagaHotelConsumer
    {
        private RabbitMqOptions _rabbitMqOptions;
        private IConnectionFactory _connectionFactory;
        private ILogger<SagaHotelConsumer> _logger;

        public SagaHotelConsumer(IOptions<RabbitMqOptions> options, IConnectionFactory connectionFactory,
            ILogger<SagaHotelConsumer> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
            _rabbitMqOptions = options.Value;
        }

        public void Consume(string queueName)
        {
            var factory = new ConnectionFactory() {HostName = "45.118.148.55", UserName = "guest", Password = "guest"};

            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            var basicConsumer = new EventingBasicConsumer(channel);
            basicConsumer.Received += (sender, args) =>
            {
                try
                {
                    var baseMessageConsumer = GetConsumer(args.BasicProperties.Headers, args.Body);
                    baseMessageConsumer.Consume(args.BasicProperties.Headers, args.Body);
                    channel.BasicAck(args.DeliveryTag,false);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                finally
                {
                    channel.BasicAck(args.DeliveryTag, false);    
                }
            };
            channel.BasicConsume(queueName, false, basicConsumer);
            channel.CallbackException += (sender, args) =>
            {
                channel.Dispose();
                Consume(queueName);
            };
        }

        private BaseMessageConsumer GetConsumer(IDictionary<string, object> header, byte[] data)
        {
            return new HotelConsumer();
        }
    }

    public abstract class BaseMessageConsumer
    {
        public abstract void Consume(IDictionary<string, object> header, byte[] data);
    }

    public class HotelConsumer : BaseMessageConsumer
    {
        public override void Consume(IDictionary<string, object> header, byte[] data)
        {
            ConsumeCore(header, data);
        }

        private void ConsumeCore(IDictionary<string, object> header, byte[] data)
        {
            var sagaCommandContextContract = header.ToObject<SagaCommandContextContract>();
            var messageType = Type.GetType(sagaCommandContextContract.MessageType);
            var bookHotelMsg = (BookHotelMsg)JsonConvert.DeserializeObject(Encoding.Unicode.GetString(data),messageType);
            var reply = new BookHotelMsgReply()
            {
                HotelId = bookHotelMsg.HotelId,
                SagaId = bookHotelMsg.SagaId
            };
            SendReplyMessage(sagaCommandContextContract, Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(reply)));
        }

        private void SendReplyMessage(SagaCommandContextContract context, byte[] data)
        {
            var factory = new ConnectionFactory() {HostName = "45.118.148.55", UserName = "guest", Password = "guest"};
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel(); 
            channel.ExchangeDeclare(context.ReplyMessageType, "direct", true, false, null);
            channel.QueueDeclare(context.ReplyAddress, true, false, false, null);
            channel.QueueBind(context.ReplyAddress, context.ReplyMessageType, "", null);
            channel.BasicPublish(context.ReplyMessageType, "", true, new BasicProperties()
            {
                Headers = context.ToDictionary(),
                Persistent = true,
            }, data);
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSagaConsumer(this IServiceCollection serviceCollection,
            IConfiguration configuration)
        {
            serviceCollection.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMQ"));
            serviceCollection.AddSingleton<IConnectionFactory>(cfg =>
            {
                var rabbitMqOptions = cfg.GetService<IOptions<RabbitMqOptions>>().Value;
                return new ConnectionFactory()
                {
                    Endpoint = new AmqpTcpEndpoint(rabbitMqOptions.Server),
                    Password = rabbitMqOptions.Password,
                    UserName = rabbitMqOptions.UserName
                };
            });
            serviceCollection.AddSingleton<SagaHotelConsumer>();
            return serviceCollection;
        }
    }
}