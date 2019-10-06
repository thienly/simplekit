namespace Test.SimpleKit.Domain.SeedDb.Events
{
    public class BankAccountAddedEvent : PersonEvent
    {
        public string BankName { get; set; }
    }
}