using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Grpc.Core;
using Xunit;

namespace ProductServiceTests.UnitTests
{
    internal class AsyncStreamRequestReader<T> : IAsyncStreamReader<T>
    {
        private readonly Channel<T> _channel;
        private readonly ServerCallContext _serverCallContext;

        public AsyncStreamRequestReader(ServerCallContext serverCallContext)
        {
            _channel = System.Threading.Channels.Channel.CreateUnbounded<T>();
            _serverCallContext = serverCallContext;
        }
        public void AddMessage(T message)
        {
            if (!_channel.Writer.TryWrite(message))
            {
                throw new InvalidOperationException("Unable to write message.");
            }
        }
        public void Complete()
        {
            _channel.Writer.Complete();
        }
        public async Task<bool> MoveNext(CancellationToken cancellationToken)
        {
            _serverCallContext.CancellationToken.ThrowIfCancellationRequested();
            if (await _channel.Reader.WaitToReadAsync(cancellationToken))
            {
                _channel.Reader.TryRead(out var message);;
                Current = message;
                return true;
            }
            else
            {
                Current = default(T);
                return false;
            }
        }

        public T Current { get; private set; }
    }

    
}