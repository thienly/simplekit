using SimpleKit.Domain;

namespace Test.SimpleKit.Domain.DataModels
{
    internal class Person : EntityWithId<int>
    {
        public Person(int id) : base(id)
        {
            Id = id;
        }
            
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}