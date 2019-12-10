using System;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Test.SimpleKit.Base
{
    public class UnitTestLogger : ILogger, IDisposable
    {
        private ITestOutputHelper _testOutputHelper;

        public UnitTestLogger(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var s = formatter(state,exception);
            _testOutputHelper.WriteLine(s);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return new UnitTestLogger(_testOutputHelper);
        }

        public void Dispose()
        {
            _testOutputHelper = null;
        }
    }
}