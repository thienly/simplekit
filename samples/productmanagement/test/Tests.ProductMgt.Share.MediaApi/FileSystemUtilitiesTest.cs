using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ProductMgt.Shared.MediaApi;
using Xunit;
using Xunit.Abstractions;

namespace Tests.ProductMgt.Share.MediaApi
{
    public class FileSystemUtilitiesTest
    {
        private IServiceProvider _provider;
        private ITestOutputHelper _testOutputHelper;
        public FileSystemUtilitiesTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            IServiceCollection serviceCollection = new ServiceCollection();
            var cfgBuilder = new ConfigurationBuilder();
            cfgBuilder.AddJsonFile("appsettings.json");
            IConfiguration cfg  = cfgBuilder.Build();
            serviceCollection.Configure<FileSystemOptions>(cfg.GetSection("FileSystemOptions"));
            serviceCollection.AddSingleton<IFileSystemUtilities, FileSystemUtilities>();
            _provider = serviceCollection.BuildServiceProvider();
        }
        [Fact]
        public void Return_true_if_option_not_null()
        {
            var service = _provider.GetService<IOptions<FileSystemOptions>>();
            var fileSystemOptions = service.Value;
            Assert.NotNull(fileSystemOptions.Endpoint);
        }

        [Fact]
        public async Task Return_true_if_can_upload_to_minio()
        {
            var fileSystemUtilities = _provider.GetService<IFileSystemUtilities>();
            byte[] datas = File.ReadAllBytes("es.png");
            var fileSystemResponse = await fileSystemUtilities.Upload("product", "product_test", datas, "image/png");
            Assert.Equal(fileSystemResponse.ErrorMessage,string.Empty);
        }
    }
}