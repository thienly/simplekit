namespace Test.SimpleKit.Repository.EfCore.SeedDb.Events
{
    public class BankAccountAddedEvent : PersonEvent
    {
        public string BankName { get; set; }
    }
}