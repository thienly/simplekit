using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SimpleKit.Domain.Events;
using Test.SimpleKit.Repository.EfCore.Base.SeedDb.Events;

namespace Test.SimpleKit.Repository.EfCore.Base.SeedDb.EventHandlers
{
    public class AddressRegisteredEventHandler : IDomainEventHandler<AddressRegisteredEvent>
    {
        private DbContext _context;

        public AddressRegisteredEventHandler(DbContext context)
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
            var address = _context.Entry(person.PermenantAddress);
            address.Property("PersonId").CurrentValue = person.Id;
            address.State = EntityState.Added;
            return Task.CompletedTask;
        }
    }
}