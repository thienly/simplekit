using System;
using AutoFixture;
using FluentAssertions;
using SimpleKit.Domain.Entities;
using Test.SimpleKit.Base;
using Xunit;
using Xunit.Abstractions;

namespace Test.SimpleKit.Domain
{
    public class DataTest : AggregateRootBase
    {
        public DataTest(Guid id): base(id)
        {
            
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
    public class AggregateRootTest : SimpleKitTestBase
    {
        private ITestOutputHelper _testOutputHelper;

        public AggregateRootTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }
        [Fact]
        public void Test_If_null_is_not_equal_with_identity()
        {
            var guidId = Guid.NewGuid();
            var dataTest = Fixture.Build<DataTest>().FromSeed(x => new DataTest(guidId)).Create<DataTest>();
            Assert.True(dataTest != null);
        }
        [Fact]
        public void Test_If_null_is_not_equal_with_identity2()
        {
            var guidId = Guid.NewGuid();
            var dataTest = Fixture.Build<DataTest>().FromSeed(x => new DataTest(guidId)).Create<DataTest>();
            Assert.True(null != dataTest);
        }
        [Fact]
        public void Test_If_two_identities_equal_if_id_equal()
        {
            var guidId = Guid.NewGuid();
            var dataTest = Fixture.Build<DataTest>().FromSeed(x => new DataTest(guidId)).Create<DataTest>();
            var dataTest1 = Fixture.Build<DataTest>().FromSeed(x => new DataTest(guidId)).Create<DataTest>();
            Assert.True(dataTest == dataTest1);
        }
        [Fact]
        public void Test_If_two_identities_not_equal_if_id_is_not_equal()
        {
            var guidId = Guid.NewGuid();
            var secondGuid = Guid.NewGuid();
            var dataTest = Fixture.Build<DataTest>().FromSeed(x => new DataTest(guidId)).Create<DataTest>();
            var dataTest1 = Fixture.Build<DataTest>().FromSeed(x => new DataTest(secondGuid)).Create<DataTest>();
            Assert.True(dataTest != dataTest1);
        }
        
    }
}