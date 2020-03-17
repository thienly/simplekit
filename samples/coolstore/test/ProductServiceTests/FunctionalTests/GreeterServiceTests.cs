using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Http;
using Grpc.Core;
using Grpc.Core.Utils;
using Grpc.Net.Client;
using Xunit;

namespace ProductServiceTests.FunctionalTests
{
    public class GreeterServiceTests : FunctionalTestBase
    {
        private HttpClient _httpClient = new HttpClient();
        [Fact]
        public async Task SayHelloUnaryTest()
        {
            var client = new TestSvc.TestSvcClient(base.Channel);
            var response = await client.SayHelloUnaryAsync(new HelloRequest()
            {
                Name = "Joe"
            });
            // Assert
            Assert.Equal("Hello Joe", response.Message);
        }
        [Fact]
        public async Task SayHelloUnaryTest_Withtimeout()
        {
            
            var client = new TestSvc.TestSvcClient(CreateChannel(new GrpcChannelOptions()
            {
                HttpClient = _httpClient,
                DisposeHttpClient = true,
            }));
            _httpClient.Timeout = TimeSpan.FromMilliseconds(5000);
            
            var response = await client.SayHelloUnaryAsync(new HelloRequest()
            {
                Name = "Joe"
            });
            // Assert
            Assert.Equal("Hello Joe", response.Message);
        }
        [Fact]
        public async Task SayHelloClientStreamingTest()
        {
            var client = new TestSvc.TestSvcClient(base.Channel);
            var clientStreaming =  client.SayHelloClientStreaming();
            await clientStreaming.RequestStream.WriteAsync(new HelloRequest()
            {
                Name = "A"
            });
            await clientStreaming.RequestStream.WriteAsync(new HelloRequest()
            {
                Name = "B"
            });
            await clientStreaming.RequestStream.WriteAsync(new HelloRequest()
            {
                Name = "C"
            });
            await clientStreaming.RequestStream.CompleteAsync();
            var clientStreamingResponseAsync = await clientStreaming.ResponseAsync;
            // Assert
            Assert.Equal("A,B,C", clientStreamingResponseAsync.Message);
        }
        [Fact]
        public async Task SayHelloServerStreamingTest()
        {
            var cts = new CancellationTokenSource();
            var client = new TestSvc.TestSvcClient(base.Channel);
            bool isCancelled = false;
            var lst = new List<string>();
            using(var serverStreaming = client.SayHelloServerStreaming(new HelloRequest()
            {
                Name = "Joe"
            }, cancellationToken: cts.Token))
            {
                await Task.Delay(2000);
                try
                {
                    while (await serverStreaming.ResponseStream.MoveNext(CancellationToken.None))
                    {
                        lst.Add(serverStreaming.ResponseStream.Current.Message);
                        cts.Cancel();
                    }
                }
                catch (Exception e)
                {
                    isCancelled = true;
                }
            }
            Assert.NotEmpty(lst);
            Assert.True(isCancelled == true);
        }
        [Fact]
        public async Task SayHelloBidirectionalStreamingTest()
        {
            var cts = new CancellationTokenSource();
            var client = new TestSvc.TestSvcClient(base.Channel);
            bool isCancelled = false;
            string message = string.Empty;
            var lst = new List<string>();
            var names = new[] {"A", "B", "C"};
            using (var bidirectionalStreaming = client.SayHelloBidirectionalStreaming(cancellationToken: cts.Token))
            {
                foreach (var name in names)
                {
                    await bidirectionalStreaming.RequestStream.WriteAsync(new HelloRequest()
                    {
                         Name = name
                    });
                    Assert.True(await bidirectionalStreaming.ResponseStream.MoveNext(CancellationToken.None));
                    lst.Add(bidirectionalStreaming.ResponseStream.Current.Message);
                }

                await bidirectionalStreaming.RequestStream.CompleteAsync();
            }
            Assert.NotEmpty(lst);
        }
    }
}