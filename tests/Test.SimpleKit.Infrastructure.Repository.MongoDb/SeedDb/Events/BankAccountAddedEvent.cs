namespace Test.SimpleKit.Infrastructure.Repository.MongoDb.SeedDb.Events
{
    public class BankAccountAddedEvent : PersonEvent
    {
        public string BankName { get; set; }
    }
}