using System;
using System.Collections.Generic;
using SimpleKit.Domain;

namespace Test.SimpleKit.Repository.EfCore.SeedDb
{
    public class Person: EntityWithId<int>
    {
        public Person(int id, string name) : base(id)
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
        }
        public IList<BankAccount> BankAccounts { get; private set; }

        public void AddBankAccount(BankAccount bankAccount)
        {
            BankAccounts.Add(bankAccount);
        }
        
    }

    public class Address : ValueObject<Address>
    {
        public string AddressNumber { get; set; }
        public string Street { get; set; }
        public string Ward { get; set; }
    }

    public class BankAccount : Entity
    {
        public string BankName { get; private set; }

        public BankAccount(Guid id, string bankName) : base(id)
        {
            Id = id;
            BankName = bankName;
        }
        
    }
}