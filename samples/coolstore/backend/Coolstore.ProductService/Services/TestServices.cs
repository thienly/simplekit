using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Coolstore.Services
{
    public class TestServices : TestSvc.TestSvcBase
    {
        private ILogger<TestServices> _logger;

        public TestServices(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<TestServices>();
        }

        public override async Task<HelloReply> SayHelloUnary(HelloRequest request, ServerCallContext context)
        {
            await Task.Delay(10000);
            _logger.LogInformation($"Sending hello to {request.Name}");
            return new HelloReply { Message = "Hello " + request.Name };
        }

        public override async Task SayHelloServerStreaming(HelloRequest request, IServerStreamWriter<HelloReply> responseStream, ServerCallContext context)
        {
            int i = 0;
            while (!context.CancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"Sending hello to {request.Name + i}");
                await responseStream.WriteAsync(new HelloReply()
                {
                    Message = $"Sending hello to {request.Name + i}"
                });
                i++;
                await Task.Delay(1000);
            }
        }

        public override async Task<HelloReply> SayHelloClientStreaming(IAsyncStreamReader<HelloRequest> requestStream, ServerCallContext context)
        {
            var names = new List<string>();
            await foreach (var request in requestStream.ReadAllAsync())
            {
                names.Add(request.Name);
            }

            return new HelloReply()
            {
                Message = string.Join(",",names)
            };
        }

        public override async Task SayHelloBidirectionalStreaming(IAsyncStreamReader<HelloRequest> requestStream, IServerStreamWriter<HelloReply> responseStream,
            ServerCallContext context)
        {
            await foreach (var message in requestStream.ReadAllAsync())
            {
                await responseStream.WriteAsync(new HelloReply { Message = "Hello " + message.Name });
            }
        }
    }
}