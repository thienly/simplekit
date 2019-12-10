using SimpleKit.Domain;

namespace Test.SimpleKit.Repository.EfCore.Base.SeedDb
{
    public class Address : ValueObject<Address>
    {
        public string AddressNumber { get; set; }
        public string Street { get; set; }
        public string Ward { get; set; }
    }
}