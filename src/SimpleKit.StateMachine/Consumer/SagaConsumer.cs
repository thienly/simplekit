using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using SimpleKit.StateMachine.Definitions;

namespace SimpleKit.StateMachine.Consumer
{
    public class SagaConsumer
    {
        private readonly string _kafkaHost;
        private IEnumerable<ISagaCommandEndpointBuilder> _endpointBuilders;
        private ILogger<SagaConsumer> _logger = NullLogger<SagaConsumer>.Instance;
        private ISagaPersistence _sagaPersistence;
        private readonly SagaStepDefinitionFactory _factory;

        public SagaConsumer(string kafkaHost,IEnumerable<ISagaCommandEndpointBuilder> endpointBuilders, ISagaPersistence sagaPersistence,SagaStepDefinitionFactory factory, ILogger<SagaConsumer> logger = null)
        {
            _kafkaHost = kafkaHost;
            _endpointBuilders = endpointBuilders;
            _sagaPersistence = sagaPersistence;
            _factory = factory;
            if (logger != null)
            {
                _logger = logger;
            }
        }

        public void Consume(ISagaReplyCommand command)
        {
            Process(command);
        }
        public void Consume()
        {
            var allReplyChannels = _endpointBuilders.SelectMany(x => x.GetReplyChannel()).ToList();
            var config = new ConsumerConfig
            {
                BootstrapServers = _kafkaHost,
                EnableAutoCommit = false,
                StatisticsIntervalMs = 5000,
                SessionTimeoutMs = 6000,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnablePartitionEof = true,
                GroupId = "OrderGroup"
            };
            using (var consumer = new ConsumerBuilder<Ignore, string>(config)
                // Note: All handlers are called on the main .Consume thread.
                .SetErrorHandler((_, e) => _logger.LogError( $"Error: {e.Reason}"))
                .SetStatisticsHandler((_, json) => _logger.LogInformation($"Statistics: {json}"))
                .SetPartitionsAssignedHandler((c, partitions) =>
                {
                    _logger.LogInformation($"Assigned partitions: [{string.Join(", ", partitions)}]");
                    // possibly manually specify start offsets or override the partition assignment provided by
                    // the consumer group by returning a list of topic/partition/offsets to assign to, e.g.:
                    // 
                    // return partitions.Select(tp => new TopicPartitionOffset(tp, externalOffsets[tp]));
                })
                .SetPartitionsRevokedHandler((c, partitions) =>
                {
                    _logger.LogInformation($"Revoking assignment: [{string.Join(", ", partitions)}]");
                })
                .Build())
            {
                consumer.Subscribe(allReplyChannels);

                try
                {
                    while (true)
                    {
                        try
                        {
                            var consumeResult = consumer.Consume();
                            if (consumeResult.IsPartitionEOF)
                            {
                                Console.WriteLine(
                                    $"Reached end of topic {consumeResult.Topic}, partition {consumeResult.Partition}, offset {consumeResult.Offset}.");

                                continue;
                            }

                            var sagaCommandReply = JsonConvert.DeserializeObject<ISagaReplyCommand>(consumeResult.Value,
                                new JsonSerializerSettings()
                                {
                                    TypeNameHandling = TypeNameHandling.All,
                                    TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full
                                });
                            Process(sagaCommandReply);
                            _logger.LogInformation($"Received message at {consumeResult.TopicPartitionOffset}: {consumeResult.Value}");
                             // The Commit method sends a "commit offsets" request to the Kafka
                             // cluster and synchronously waits for the response. This is very
                             // slow compared to the rate at which the consumer is capable of
                             // consuming messages. A high performance application will typically
                             // commit offsets relatively infrequently and be designed handle
                             // duplicate messages in the event of failure.
                            consumer.Commit();
                        }
                        catch (ConsumeException e)
                        {
                            _logger.LogError($"Consume error: {e.Error.Reason}");
                        }
                        catch (Exception e)
                        {
                            _logger.LogError($"Consume error: {e.Message}");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    _logger.LogError("Closing consumer.");
                    consumer.Close();
                }
            }
        }

        private void Process(ISagaReplyCommand sagaReply)
        {
            var sagaStateProxy = _sagaPersistence.Load(sagaReply.SagaId);
            // Apply the change to the state
            var sagaState = sagaStateProxy.SagaState;
            var methodInfos = sagaState.GetType().GetMethods().ToList();
            var methodInfo = methodInfos.Single(x =>
                x.GetParameters().Length == 1 &&
                typeof(ISagaReplyCommand).IsAssignableFrom(x.GetParameters()[0].ParameterType));
            methodInfo.Invoke(sagaState, new[] {sagaReply});
            // Get the definition
            var definition = _factory(sagaStateProxy.SagaDefinitionType);
            if (sagaReply.IsSuccess)
            {
                //GetNextStep(string currentStep)
                var method =
                    sagaStateProxy.SagaDefinitionType.GetMethod("GetNextStepName",
                        BindingFlags.Instance | BindingFlags.Public);
                var nextState = method.Invoke(definition,new []{sagaStateProxy.CurrentState});
                if (string.IsNullOrEmpty(nextState.ToString()))
                {
                    sagaStateProxy.MarkCompleted();
                }
                else
                {
                    sagaStateProxy.MarkUncompleted();
                    sagaStateProxy.SetNextState(nextState.ToString());
                }
            }
            else
            {
                //GetNextStep(string currentStep)
                var method =
                    sagaStateProxy.SagaDefinitionType.GetMethod("GetPreviousStepName",
                        BindingFlags.Instance | BindingFlags.Public);
                var previouseState = method.Invoke(definition,new []{sagaStateProxy.CurrentState});
                if (string.IsNullOrEmpty(previouseState.ToString()))
                {
                    sagaStateProxy.MarkCompleted();
                }
                else
                {
                    sagaStateProxy.MarkUncompleted();
                    sagaStateProxy.SetNextState(previouseState.ToString());
                }
            }
            sagaStateProxy.IncreaseVersion();
            _sagaPersistence.Save(sagaStateProxy);
        }
    }
}