using System;
using System.Collections.Generic;
using AutoMapper;
using AutoMapper.Configuration;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var mapper = new Mapper(new MapperConfiguration(cfg => { cfg.CreateMap<Person, PersonDto>(); }));
            var p = new PersonDto()
            {
                Name ="THIEN"
            };
            var p1 = new Person.Member();
            p1.Age = 10;
            Console.Write(p1.Age);
        }
    }

    public class Person
    {
        public string Name { get; set; }
        public ICollection<Member> Members { get; set; }
        public class Member
        {
            public int Age { get; set; }
        }
        
    }

    public class PersonDto
    {
        public string Name { get; set; }
        public Person.Member Member { get; set; }
    }
}