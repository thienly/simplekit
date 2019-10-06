namespace SampleApiWithEfCore.Models.Events
{
    public class BankAccountAddedEvent : PersonEvent
    {
        public string BankName { get; set; }
    }
}