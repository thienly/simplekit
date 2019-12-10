using System.Threading.Tasks;
using ProductMgt.Domain;
using ProductMgt.Domain.Events;
using SimpleKit.Domain.Events;
using SimpleKit.Domain.Repositories;

namespace ProductMgt.DomainEventHandlers
{
    public class ProductAddedEventHandler : IDomainEventHandler<ProductAddedEvent>
    {
        private IRepository<Product> _repository;

        public ProductAddedEventHandler(IRepository<Product> repository)
        {
            _repository = repository;
        }

        public Task Handle(ProductAddedEvent @event)
        {
            return Task.CompletedTask;
        }
    }
}