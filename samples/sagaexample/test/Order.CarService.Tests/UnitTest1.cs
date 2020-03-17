using System;
using Newtonsoft.Json.Serialization;
using Order.CarService.Domains;
using ProtoBuf;
using Xunit;
using Xunit.Abstractions;

namespace Order.CarService.Tests
{
    public class UnitTest1
    {
        private ITestOutputHelper _outputHelper;

        public UnitTest1(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        [Fact]
        public void Test1()
        {
            var proto = Serializer.GetProto<Car>();
            _outputHelper.WriteLine(proto);
        }
    }
}