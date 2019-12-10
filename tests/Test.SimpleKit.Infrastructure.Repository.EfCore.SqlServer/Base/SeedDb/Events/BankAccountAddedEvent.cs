namespace Test.SimpleKit.Repository.EfCore.Base.SeedDb.Events
{
    public class BankAccountAddedEvent : PersonEvent
    {
        public string BankName { get; set; }
    }
}