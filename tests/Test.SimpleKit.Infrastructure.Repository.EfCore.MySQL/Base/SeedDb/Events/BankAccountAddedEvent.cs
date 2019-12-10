namespace Test.SimpleKit.Infrastructure.Repository.EfCore.MySQL.Base.SeedDb.Events
{
    public class BankAccountAddedEvent : PersonEvent
    {
        public string BankName { get; set; }
    }
}