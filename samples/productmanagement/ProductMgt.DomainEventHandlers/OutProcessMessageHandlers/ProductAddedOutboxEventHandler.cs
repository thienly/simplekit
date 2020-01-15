using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Confluent.Kafka;
using Newtonsoft.Json;
using ProductMgt.Domain;
using ProductMgt.Domain.Events;
using ProductMgt.Infrastructure.UserContext;
using SimpleKit.Domain.Repositories;
using SimpleKit.Infrastructure.Bus.Kafka;

namespace ProductMgt.DomainEventHandlers.OutProcessMessageHandlers
{
    public class ProductAddedOutboxEventHandler : OutboxMessageEventHandler<ProductAddedEvent>
    {
        private IRepository<OutboxMessage> _outBoxRepository;
        private IUserContextFactory _userContextFactory;
        private IKafkaProducerFactory<Null, string> _kafkaProducerFactory;

        public ProductAddedOutboxEventHandler(IRepository<OutboxMessage> outBoxRepository,
            IUserContextFactory userContextFactory, IKafkaProducerFactory<Null, string> kafkaProducerFactory) : base(
            outBoxRepository, userContextFactory, kafkaProducerFactory)
        {
            _outBoxRepository = outBoxRepository;
            _userContextFactory = userContextFactory;
            _kafkaProducerFactory = kafkaProducerFactory;
        }


        protected override async Task<bool> Publish(ProductAddedEvent domainEvent)
        {
            using (var producer = _kafkaProducerFactory.Create())
            {
                var deliveryResult = await producer.ProduceAsync("product_added", new Message<Null, string>()
                {
                    Value = JsonConvert.SerializeObject(domainEvent)
                });
                return deliveryResult.TopicPartitionOffset.Offset.Value > 0;
            }
        }
    }
}