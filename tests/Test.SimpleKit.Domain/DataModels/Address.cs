using SimpleKit.Domain;

namespace Test.SimpleKit.Domain.DataModels
{
    public class Address : ValueObject<Address>
    {
        public Address(string streetName, string ward, string city)
        {
            StreetName = streetName;
            Ward = ward;
            City = city;
        }
        public string StreetName { get; private set; }
        public string Ward { get; private set; }
        public string City { get; private set; }
    }
}