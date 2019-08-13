using System;
using SimpleKit.Domain;
using Xunit;

namespace Test.SimpleKit.Domain
{
    public class UnitTest1
    {
        [Fact]
        public void Test_equality_of_two_entity()
        {
            
        }
        internal class Person : Entity<int>
        {
            public Person(int id)
            {
                Id = id;
            }
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }
    }
}