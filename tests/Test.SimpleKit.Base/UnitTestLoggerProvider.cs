using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Test.SimpleKit.Base
{
    public class UnitTestLoggerFactory : ILoggerFactory
    {
        private ILoggerProvider _loggerProvider;
        private ITestOutputHelper _testOutputHelper;

        public UnitTestLoggerFactory(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        public void Dispose()
        {
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggerProvider.CreateLogger(categoryName);
        }

        void ILoggerFactory.AddProvider(ILoggerProvider provider)
        {
            AddProvider();
        }

        public void AddProvider()
        {
            _loggerProvider = new UnitTestLoggerProvider(_testOutputHelper);
        }
    }
    public class UnitTestLoggerProvider : ILoggerProvider
    {
        private ITestOutputHelper _testOutputHelper;

        public UnitTestLoggerProvider(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        public void Dispose()
        {
            _testOutputHelper = null;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new UnitTestLogger(_testOutputHelper);
        }
    }
}