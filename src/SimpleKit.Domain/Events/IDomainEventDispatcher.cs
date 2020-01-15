using System.Threading.Tasks;

namespace SimpleKit.Domain.Events
{
    public interface IDomainEventDispatcher
    {
        void Dispatch(IDomainEvent @domainEvent);
    }
}