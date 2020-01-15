using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json;
using Order.Shared;
using SimpleKit.StateMachine.Definitions;

namespace KitchenWorker
{
    class Program
    {
        static void Main(string[] args)
        {
            Consume();
        }
        public static void Publish<T>(T data)
        {
            var config = new ProducerConfig()
            {
                BootstrapServers = "10.0.19.103:9092",
                EnableIdempotence = true
            };
            using (var producer = new ProducerBuilder<Null, string>(config)
                .SetStatisticsHandler((_, stas) => Console.WriteLine($"The statistic of kafka producer{stas}"))
                .SetLogHandler((_, message) =>
                    Console.WriteLine((
                        $"Producer log handler level {message.Level.ToString()} with message {message.Message}")))
                .Build())
            {
                producer.Produce("KitchenReply", new Message<Null, string>()
                {
                    Timestamp = Timestamp.Default,
                    Value = JsonConvert.SerializeObject(data, new JsonSerializerSettings()
                    {
                        TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full,
                        TypeNameHandling = TypeNameHandling.All

                    })
                });
                producer.Flush();
            }
        }

        public static void Consume()
        {
            var config = new ConsumerConfig()
            {
                BootstrapServers = "10.0.19.103:9092",
                IsolationLevel = IsolationLevel.ReadCommitted,
                AutoOffsetReset = AutoOffsetReset.Latest,
                EnableAutoCommit = false,
                GroupId = "groupId"
            };
            using (var consumer = new ConsumerBuilder<Null, string>(config)
                .SetErrorHandler(((_, e) =>
                    Console.WriteLine($"There is an error while consuming message with reason {e.Reason}")))
                .Build())
            {
                consumer.Subscribe("Order_Kitchen");

                while (true)
                {
                    var consumeResult = consumer.Consume();
                    var sagaReplyCommand = JsonConvert.DeserializeObject<AskingKitchenCommand>(consumeResult.Value);
                    Publish(new KitchenReplyCommand()
                    {
                        SagaId = sagaReplyCommand.OrderId,
                        OrderId = sagaReplyCommand.OrderId,
                        IsSuccess = true
                    });
                    if (consumeResult.IsPartitionEOF)
                    {
                        Console.WriteLine(
                            $"Reached end of topic {consumeResult.Topic}, partition {consumeResult.Partition}, offset {consumeResult.Offset}.");

                        continue;
                    }
                    
                    consumer.Commit();
                    Console.WriteLine(
                        $"Received message at {consumeResult.TopicPartitionOffset}: {consumeResult.Value}");
                }
            }
        }
    }
}