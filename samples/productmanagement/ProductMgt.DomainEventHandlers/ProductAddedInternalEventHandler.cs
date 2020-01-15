using System.Threading.Tasks;
using ProductMgt.Domain.Events;
using SimpleKit.Domain.Events;

namespace ProductMgt.DomainEventHandlers
{
    public class ProductAddedInternalEventHandler : IDomainEventHandler<ProductAddedEvent>
    {
        public Task Handle(ProductAddedEvent @event)
        {
            return Task.CompletedTask;
        }
    }
}