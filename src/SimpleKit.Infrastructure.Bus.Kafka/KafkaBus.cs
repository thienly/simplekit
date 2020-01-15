using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using SimpleKit.Domain.Events;
using SimpleKit.Infrastructure.Bus.Kafka.Interfaces;

namespace SimpleKit.Infrastructure.Bus.Kafka
{
    public interface IKafkaProducerFactory<TKey, TValue>
    {
        IProducer<TKey, TValue> Create();
    }

    public class KafkaProducerFactory<TKey, TValue> : IKafkaProducerFactory<TKey, TValue>
    {
        private KafkaOptions _kafkaOptions;
        public ILogger<KafkaProducerFactory<TKey, TValue>> Logger { get; set; }

        private static ConcurrentDictionary<string, IProducer<TKey, TValue>> _cachedItems =
            new ConcurrentDictionary<string, IProducer<TKey, TValue>>();

        public KafkaProducerFactory(KafkaOptions kafkaOptions)
        {
            _kafkaOptions = kafkaOptions;
            Logger = NullLogger<KafkaProducerFactory<TKey, TValue>>.Instance;
        }

        public IProducer<TKey, TValue> Create()
        {
            var config = new ProducerConfig()
            {
                MessageTimeoutMs = 500,
                EnableIdempotence = true,
                BootstrapServers = _kafkaOptions.KafkaHost
            };

            var producer = new ProducerBuilder<TKey, TValue>(config)
                .SetErrorHandler((_producers, error) =>
                {
                    Logger.LogError(
                        $"There is an error when publising message to kafka with reason {error.Reason}");
                })
                .SetLogHandler((_, log) => Logger.LogInformation($"Publishing information {log.Message}"))
                .SetStatisticsHandler((_, sta) => Logger.LogInformation($"Statistics information {sta}"))
                .Build();


            return producer;
        }
    }

    public class KafkaBus : IKafkaBus
    {
        private KafkaOptions _options;
        private ISubscriptionManager _subscriptionManager;
        private IIntegrationEventAggregation _integrationEventAggregation;
        public ILogger<KafkaBus> Logger { get; set; }
        private IntegrationEventHandlerFactory _eventHandlerFactory;

        public KafkaBus(KafkaOptions options, ISubscriptionManager subscriptionManager,
            IntegrationEventHandlerFactory eventHandlerFactory)
        {
            _options = options;
            _subscriptionManager = subscriptionManager;
            _eventHandlerFactory = eventHandlerFactory;
            Logger = NullLogger<KafkaBus>.Instance;
        }

        public IReadOnlyList<string> GetTopicsForConsuming(params Assembly[] scannedAssemblies)
        {
            var lst = _subscriptionManager.Subscriptions.Where(x => !x.IsDynamic).Select(x => x.EventName).ToList();
            var staticTopics = lst.Select(Type.GetType)
                .SelectMany(x => x.GetCustomAttributes<KafkaTopicAttribute>().SelectMany(t => t.TopicName)).Distinct()
                .ToList();
            var dynamicTopic = _subscriptionManager.Subscriptions.Where(x => x.IsDynamic).Select(x => x.EventName)
                .Distinct().ToList();
            return staticTopics.Concat(dynamicTopic).ToList();
        }

        public void Consume()
        {
            try
            {
                var config = new ConsumerConfig()
                {
                    BootstrapServers = _options.KafkaHost,
                    IsolationLevel = IsolationLevel.ReadCommitted,
                    AutoOffsetReset = AutoOffsetReset.Latest,
                    GroupId = _options.GroupId,
                    EnableAutoCommit = true
                };
                using (var consumer = new ConsumerBuilder<Null, string>(config)
                    .SetErrorHandler(((_, e) =>
                        Logger.LogError($"There is an error while consuming message with reason {e.Reason}")))
                    .SetStatisticsHandler((_,
                        x) =>
                    {
                        Logger.LogInformation(
                            $"Consumer that belong to group {_options.GroupId} consuming message {x}");
                    }).SetOffsetsCommittedHandler((_, offset) =>
                        Logger.LogInformation(
                            $"Consumer that belong to group {_options.GroupId} committing offset {JsonConvert.SerializeObject(offset.Offsets)}"))
                    .SetPartitionsAssignedHandler((_, topics) =>
                        Logger.LogInformation(
                            $"Consumer is assigning to partition {JsonConvert.SerializeObject(topics)}"))
                    .SetPartitionsRevokedHandler((_, topics) =>
                        Logger.LogInformation(
                            $"Consumer is re-assigning to partition {JsonConvert.SerializeObject(topics)}"))
                    .Build())
                {
                    var topics = GetTopicsForConsuming();
                    consumer.Subscribe(topics);
                    try
                    {
                        while (true)
                        {
                            var consumeResult = consumer.Consume(TimeSpan.FromSeconds(2));
                            if (consumeResult.IsPartitionEOF)
                            {
                                Logger.LogInformation(
                                    $"Reached end of topic {consumeResult.Topic}, partition {consumeResult.Partition}, offset {consumeResult.Offset}.");

                                continue;
                            }

                            Logger.LogInformation(
                                $"Received message at {consumeResult.TopicPartitionOffset}: {consumeResult.Value}");
                        }
                    }
                    catch (KafkaConsumerException e)
                    {
                        Logger.LogError(e,
                            $"There is an exception while consuming message. So that consumer is closed");
                        consumer.Close();
                    }
                }
            }
            catch (KafkaConsumerException e)
            {
                Logger.LogError(e, $"There is an exception while building configuration for consumer ");
            }
        }

        private async Task ProcessMessage(string eventName, string body)
        {
            foreach (var sb in _subscriptionManager.Subscriptions)
            {
                if (typeof(IDynamicIntegrationEventHandler).IsAssignableFrom(sb.EventHandler))
                {
                    var handler = (IDynamicIntegrationEventHandler) _eventHandlerFactory(sb.EventHandler);
                    var t = (Task) handler.Handle(body);
                    await t;
                }
                else
                {
                    var typeEvent = sb.EventHandler.GetGenericTypeDefinition();
                    var interfaceHandler = typeof(IIntegrationEventHandler<>).MakeGenericType(typeEvent);
                    var handler = _eventHandlerFactory(interfaceHandler);
                    var methodInfo =
                        sb.EventHandler.GetMethod("Handle", BindingFlags.Instance | BindingFlags.Public);
                    var data = JsonConvert.DeserializeObject(body, typeEvent);
                    var task = (Task) methodInfo.Invoke(handler, new[] {data});
                    await task;
                }
            }
        }

        private IReadOnlyList<string> GetTopicsFromType(Type t)
        {
            var listTopic = new List<string>();
            if (t.GetCustomAttributes<KafkaTopicAttribute>().Any())
            {
                listTopic.AddRange(t.GetCustomAttributes<KafkaTopicAttribute>().SelectMany(x => x.TopicName).ToList());
            }
            else
            {
                throw new KafkaProducerException(
                    $"The type {t.FullName} does not have attribute {typeof(KafkaTopicAttribute).FullName}");
            }

            return listTopic;
        }

        /// <summary>
        /// Use this method to publish a record to kafka with default topic(in kafka options). In case want to publish to multiple topic, the class must have
        /// KafkaTopicAttribute <code> [KafkaTopicAttribute("employee","role")]</code>
        /// </summary>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task Publish<T>(T data) where T : IntegrationEvent
        {
            try
            {
                var config = new ProducerConfig()
                {
                    BootstrapServers = _options.KafkaHost,
                    EnableIdempotence = true
                };
                using (var producer = new ProducerBuilder<Null, string>(config)
                    .SetStatisticsHandler((_, stas) => Logger.LogInformation($"The statistic of kafka producer{stas}"))
                    .SetLogHandler((_, message) =>
                        Logger.LogInformation(
                            $"Producer log handler level {message.Level.ToString()} with message {message.Message}"))
                    .Build())
                {
                    var listTopic = GetTopicsFromType(typeof(T));
                    Logger.LogInformation($"Start publishing message to topic: {string.Join(",", listTopic)}");
                    foreach (var topic in listTopic)
                    {
                        await producer.ProduceAsync(topic, new Message<Null, string>()
                        {
                            Timestamp = Timestamp.Default,
                            Value = JsonConvert.SerializeObject(data)
                        }).ContinueWith(t1 =>
                        {
                            if (t1.IsCompletedSuccessfully)
                            {
                                Logger.LogInformation(
                                    $"Publishing message successfully to topic {topic}, partition {t1.Result.Partition.Value} and offset {t1.Result.Offset.Value}");
                            }
                            else
                            {
                                Logger.LogError(
                                    $"Publishing message unsuccessfully to topic {topic}");
                            }
                        });
                    }
                }
            }
            catch (ProduceException<Null, string> e)
            {
                Logger.LogError(e,
                    $"Failed to publish message to kafka {e.Message} and code {e.Error.Code}");
            }
        }
    }
}