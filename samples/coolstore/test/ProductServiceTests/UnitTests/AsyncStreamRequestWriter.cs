using System.Threading.Channels;
using System.Threading.Tasks;
using Grpc.Core;

namespace ProductServiceTests.UnitTests
{
    internal class AsyncStreamRequestWriter<T> : IServerStreamWriter<T>
    {
        private Channel<T> _channel;
        private ServerCallContext _serverCallContext;
        public AsyncStreamRequestWriter(ServerCallContext serverCallContext)
        {
            _serverCallContext = serverCallContext;
            _channel = System.Threading.Channels.Channel.CreateUnbounded<T>();
            ChannelReader = _channel.Reader;
            ChannelWriter = _channel.Writer;
        }
        public async Task WriteAsync(T message)
        {
            await _channel.Writer.WaitToWriteAsync();
            await _channel.Writer.WriteAsync(message);
            
        }

        public ChannelReader<T> ChannelReader { get; private set; }
        public ChannelWriter<T> ChannelWriter { get; private set; } 

        public WriteOptions WriteOptions { get; set; }
    }
}