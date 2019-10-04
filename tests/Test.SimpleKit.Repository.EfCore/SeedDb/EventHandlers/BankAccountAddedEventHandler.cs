using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SimpleKit.Domain.Events;
using Test.SimpleKit.Repository.EfCore.SeedDb.Events;

namespace Test.SimpleKit.Repository.EfCore.SeedDb.EventHandlers
{
    public class BankAccountAddedEventHandler : IDomainEventHandler<BankAccountAddedEvent>
    {
        private Microsoft.EntityFrameworkCore.DbContext _dbContext;

        public BankAccountAddedEventHandler(Microsoft.EntityFrameworkCore.DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task Handle(BankAccountAddedEvent @event)
        {
            var bankAccounts = _dbContext.Set<BankAccount>();
            var newBankAccount = new BankAccount(@event.BankName);
            newBankAccount.BelongToPerson(@event.Person);
            _dbContext.Entry(newBankAccount).State = EntityState.Added;
            return Task.CompletedTask;
        }
    }
}