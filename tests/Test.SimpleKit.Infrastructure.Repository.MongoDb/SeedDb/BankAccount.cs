using System;
using SimpleKit.Domain.Entities;

namespace Test.SimpleKit.Infrastructure.Repository.MongoDb.SeedDb
{
    public class BankAccount : Entity
    {
        public string BankName { get; private set; }

        public BankAccount(string bankName)
        {
            BankName = bankName;
        }
        public BankAccount(Guid id, string bankName) : base(id)
        {
            Id = id;
            BankName = bankName;
        }

        public Person Person { get; private set; }

        public void BelongToPerson(Person person)
        {
            Person = person;
        }
    }
}