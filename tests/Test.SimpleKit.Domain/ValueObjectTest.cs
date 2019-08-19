using AutoFixture;
using Test.SimpleKit.Base;
using Test.SimpleKit.Domain.DataModels;
using Xunit;

namespace Test.SimpleKit.Domain
{
    public class ValueObjectTest : SimpleKitTestBase
    {
        [Fact]
        public void Test_equality_of_value_object()
        {
            var add1 = Fixture.Build<Address>().FromFactory(() => new Address("HVL","HCM","P1")).Create();
            var add2 = Fixture.Build<Address>().FromFactory(() => new Address("HVL","HCM","P1")).Create();
            Assert.True(add1 == add2);
        }
        [Fact]
        public void Test_equality_of_value_object_2()
        {
            var add1 = Fixture.Build<Address>().FromFactory(() => new Address("HVL","HCM","P1")).Create();
            var add2 = Fixture.Create<Address>();
            Assert.True(add1 != add2);
        }
    }
}