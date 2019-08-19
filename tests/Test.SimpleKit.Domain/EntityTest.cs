using System;
using AutoFixture;
using SimpleKit.Domain;
using Test.SimpleKit.Base;
using Test.SimpleKit.Domain.DataModels;
using Xunit;

namespace Test.SimpleKit.Domain
{
    public class EntityTest : SimpleKitTestBase
    {
        [Fact]
        public void Test_equality_of_two_entity()
        {
            var p1 = Fixture.Build<Person>().FromSeed(p => new Person(1)).Create();
            var p2 = Fixture.Build<Person>().FromSeed(p => new Person(1)).Create();
            Assert.True(p1 ==p2);
        }
        [Fact]
        public void Test_equality_of_two_entity_2()
        {
            var p1 = Fixture.Build<Person>().FromSeed(p => new Person(1)).Create();
            var p2 = Fixture.Build<Person>().FromSeed(p => new Person(2)).Create();
            Assert.True(p1 !=p2);
        }
        
    }
}