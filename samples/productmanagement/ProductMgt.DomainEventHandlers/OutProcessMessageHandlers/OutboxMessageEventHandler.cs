using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using Newtonsoft.Json;
using ProductMgt.Domain;
using ProductMgt.Infrastructure.UserContext;
using SimpleKit.Domain.Events;
using SimpleKit.Domain.Repositories;
using SimpleKit.Infrastructure.Bus.Kafka;

namespace ProductMgt.DomainEventHandlers.OutProcessMessageHandlers
{
    public abstract class OutboxMessageEventHandler<TDomainEvent> : IDomainEventHandler<TDomainEvent>
        where TDomainEvent : IDomainEvent
    {
        private IRepository<OutboxMessage> _repository;
        private IUserContextFactory _userContextFactory;
        private UserContext _userContext;
        private IKafkaProducerFactory<Null, string> _kafkaProducerFactory;

        protected OutboxMessageEventHandler(IRepository<OutboxMessage> repository,
            IUserContextFactory userContextFactory, IKafkaProducerFactory<Null, string> kafkaProducerFactory)
        {
            _repository = repository;
            _userContextFactory = userContextFactory;
            _kafkaProducerFactory = kafkaProducerFactory;
            _userContext = _userContextFactory.Create();
        }

        public async Task Handle(TDomainEvent @event)
        {
            var outboxMessage = new OutboxMessage()
            {
                Body = JsonConvert.SerializeObject(@event),
                CreatedDate = DateTime.Now,
                Type = @event.GetType(),
                CreatedBy = _userContext.UserId,
                DispatchedTime = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
            };

            int tried = 0;
            bool isSuccess = false;
            while (true)
            {
                try
                {
                    await Publish(@event);
                    isSuccess = true;
                    break;
                }
                catch (Exception exception)
                {
                    if (tried > 3)
                        break;
                    tried++;
                }
            }

            if (isSuccess)
            {
                outboxMessage.ProcessedTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            }

            await SaveOutboxMessage(outboxMessage);
        }

        private Task SaveOutboxMessage(OutboxMessage message)
        {
            return _repository.AddAsync(message);
        }

        protected abstract Task<bool> Publish(TDomainEvent @domainEvent);
    }
}