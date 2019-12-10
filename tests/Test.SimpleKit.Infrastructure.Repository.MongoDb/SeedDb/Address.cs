using SimpleKit.Domain;

namespace Test.SimpleKit.Infrastructure.Repository.MongoDb.SeedDb
{
    public class Address : ValueObject<Address>
    {
        public string AddressNumber { get; set; }
        public string Street { get; set; }
        public string Ward { get; set; }
    }
}