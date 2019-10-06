using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SampleApiWithEfCore.Models.Events;
using SimpleKit.Domain.Events;

namespace SampleApiWithEfCore.Models.EventHandlers
{
    public class AddressRegisteredEventHandler : IDomainEventHandler<AddressRegisteredEvent>
    {
        private Microsoft.EntityFrameworkCore.DbContext _context;

        public AddressRegisteredEventHandler(Microsoft.EntityFrameworkCore.DbContext context)
        {
            _context = context;
        }

        public Task Handle(AddressRegisteredEvent @event)
        {
            var person = _context.Set<Person>().Find(@event.Person.Id);
            person.RegisterAddress(new Address()
            {
                AddressNumber = @event.AddressNumber,
                Street = @event.Street,
                Ward = @event.Ward
            });
            _context.Entry(person).State = EntityState.Modified;
            return Task.CompletedTask;
        }
    }
}