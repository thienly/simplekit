using System;
using Coolstore.ProductService;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProductServiceTests.FunctionalTests.Helpers;

namespace ProductServiceTests.FunctionalTests
{
    public class FunctionalTestBase
    {
        private GrpcChannel? _channel;
        private IDisposable? _testContext;

        protected GrpcTestFixture<Startup> Fixture { get; private set; } = new GrpcTestFixture<Startup>();

        protected ILoggerFactory LoggerFactory => Fixture.LoggerFactory;

        protected GrpcChannel Channel => _channel ??= CreateChannel();

        protected GrpcChannel CreateChannel(GrpcChannelOptions options = null)
        {
            if (options != null)
                return  GrpcChannel.ForAddress(Fixture.Client.BaseAddress,options);
            return GrpcChannel.ForAddress(Fixture.Client.BaseAddress, new GrpcChannelOptions
            {
                LoggerFactory = LoggerFactory,
                HttpClient = Fixture.Client
            });
        }

        protected virtual void ConfigureServices(IServiceCollection services)
        {
        }

    }
}