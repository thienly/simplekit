using SimpleKit.Domain;

namespace Test.SimpleKit.Infrastructure.Repository.EfCore.MySQL.Base.SeedDb
{
    public class Address : ValueObject<Address>
    {
        public string AddressNumber { get; set; }
        public string Street { get; set; }
        public string Ward { get; set; }
    }
}