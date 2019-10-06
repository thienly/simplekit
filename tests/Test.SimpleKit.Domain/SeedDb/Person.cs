using System;
using System.Collections.Generic;
using SimpleKit.Domain.Entities;
using Test.SimpleKit.Domain.SeedDb.Events;

namespace Test.SimpleKit.Domain.SeedDb
{
    public class Person: AggregateRootWithId<int>
    {
        private readonly List<BankAccount> _bankAccounts = new List<BankAccount>();

        protected Person(int id, string name) : base(id)
        {
            Name = name;
        }

        public Person(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
        public Address PermenantAddress { get; private set; }

        public void RegisterAddress(Address address)
        {
            PermenantAddress = address;
            var @event = new AddressRegisteredEvent()
            {
                AddressNumber = address.AddressNumber,
                EventId = Guid.NewGuid(),
                Person = this,
                Street = address.Street,
                Ward = address.Ward
            };
            AddEvent(@event);
        }

        public IReadOnlyCollection<BankAccount> BankAccounts => _bankAccounts;

        public void AddBankAccount(BankAccount bankAccount)
        {
            _bankAccounts.Add(bankAccount);
            var @event = new BankAccountAddedEvent()
            {
                Person = this,
                BankName = bankAccount.BankName,
                EventId = Guid.NewGuid()
            };
            AddEvent(@event);
        }
        
    }
}