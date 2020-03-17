using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Coolstore.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using Xunit.Abstractions;

namespace ProductServiceTests.UnitTests
{
    public class TestServicesTest
    {
        private ITestOutputHelper _testOutputHelper;

        public TestServicesTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task SayHelloUnaryTest()
        {
            var services = new TestServices(NullLoggerFactory.Instance);
            var request = new HelloRequest()
            {
                Name = "UnitTest"
            };
            var sayHelloUnary = await services.SayHelloUnary(request,UnitTestServiceContext.Create());
            Assert.Equal(sayHelloUnary.Message, "Hello " + request.Name);
        }

        [Fact]
        public async Task SayHelloClientStreamingTest()
        {
            var svc = new TestServices(NullLoggerFactory.Instance);
            var cts = new CancellationTokenSource();
            var callContext = UnitTestServiceContext.Create(cancellationToken: cts.Token);
            var requestStream = new AsyncStreamRequestReader<HelloRequest>(callContext);
            var sayHelloClientStreaming = svc.SayHelloClientStreaming(requestStream, callContext);
            requestStream.AddMessage(new HelloRequest { Name = "James" });
            requestStream.AddMessage(new HelloRequest { Name = "Jo" });
            requestStream.AddMessage(new HelloRequest { Name = "Lee" });
            requestStream.Complete();
            
            // Assert
            var response = await sayHelloClientStreaming;
            Assert.Equal("James,Jo,Lee", response.Message);
        }
        [Fact]
        public async Task SayHelloServerStreamingTest()
        {
            var svc = new TestServices(NullLoggerFactory.Instance);
            var cts = new CancellationTokenSource();
            var callContext = UnitTestServiceContext.Create(cancellationToken: cts.Token);
            var requestStream = new AsyncStreamRequestReader<HelloRequest>(callContext);
            var request = new HelloRequest()
            {
                Name = "Message"
            };
            var streamWriter = new AsyncStreamRequestWriter<HelloReply>(callContext);
            svc.SayHelloServerStreaming(request, streamWriter, callContext);

            cts.Cancel();
            await Task.Delay(1000);
            streamWriter.ChannelWriter.Complete();
            await foreach (var item in streamWriter.ChannelReader.ReadAllAsync())
            {
                _testOutputHelper.WriteLine(item.Message);                        
            }
        }
        [Fact]
        public async Task SayHelloBidirectionalStreamingTest()
        {
            var svc = new TestServices(NullLoggerFactory.Instance);
            var cts = new CancellationTokenSource();
            var callContext = UnitTestServiceContext.Create(cancellationToken: cts.Token);
            var requestStream = new AsyncStreamRequestReader<HelloRequest>(callContext);
            var replyStream = new AsyncStreamRequestWriter<HelloReply>(callContext);
            var call = svc.SayHelloBidirectionalStreaming(requestStream, replyStream, callContext);
            requestStream.AddMessage(new HelloRequest()
            {
                Name = "Test1"
            });
            requestStream.AddMessage(new HelloRequest()
            {
                Name = "Test2"
            });
            requestStream.AddMessage(new HelloRequest()
            {
                Name = "Test3"
            });
            requestStream.Complete();
            await call;
            replyStream.ChannelWriter.Complete();
            await foreach (var item in replyStream.ChannelReader.ReadAllAsync())
            {
                _testOutputHelper.WriteLine(item.Message);
            }
            
        }

        [Fact]
        public async Task Test()
        {
            var s = new Sample();
            await foreach (var i in s.GetNames())
            {
                _testOutputHelper.WriteLine(i);
            }
        }
        
    }
    public class Sample
    {
        public async IAsyncEnumerable<string> GetNames()
        {
            var list = new List<string>();
            list.Add("A");
            list.Add("B");
            list.Add("C");
            list.Add("D");
            foreach (var i in list)
            {
                yield return i;
            }
        }
    }
}