namespace Test.SimpleKit.Repository.EfCore.SeedDb.Events
{
    public class AddressRegisteredEvent : PersonEvent
    {
        public string AddressNumber { get; set; }
        public string Street { get; set; }
        public string Ward { get; set; }
    }
}