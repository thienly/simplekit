using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SimpleKit.Domain.Events;
using Test.SimpleKit.Infrastructure.Repository.EfCore.MySQL.Base.SeedDb.Events;

namespace Test.SimpleKit.Infrastructure.Repository.EfCore.MySQL.Base.SeedDb.EventHandlers
{
    public class BankAccountAddedEventHandler : IDomainEventHandler<BankAccountAddedEvent>
    {
        private DbContext _dbContext;

        public BankAccountAddedEventHandler(DbContext dbContext)
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