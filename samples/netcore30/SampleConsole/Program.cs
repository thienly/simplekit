using System;

namespace SampleConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var p = new Person("Thien",null);
            Console.WriteLine("Person");
        }
    }
    #nullable enable
    public class Person
    {
        public Person(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DOB { get; set; }
    }
    #nullable restore
}