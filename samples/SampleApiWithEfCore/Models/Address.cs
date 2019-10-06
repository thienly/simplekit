using SimpleKit.Domain;

namespace SampleApiWithEfCore.Models
{
    public class Address : ValueObject<Address>
    {
        public string AddressNumber { get; set; }
        public string Street { get; set; }
        public string Ward { get; set; }
        public Person Person { get; set; }
    }
}