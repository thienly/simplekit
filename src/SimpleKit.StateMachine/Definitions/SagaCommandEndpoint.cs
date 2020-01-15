using System;

namespace SimpleKit.StateMachine.Definitions
{
    public abstract class SagaCommandEndpoint
    {
        public string Channel { get; set; }
        public Type ReplyDataType { get; set; }
    }

    public class NoReplyCommandEndpoint: SagaCommandEndpoint 
    {
        
    }
    public class SagaCommandEndpoint<TData> : SagaCommandEndpoint where TData: class, ISagaCommand 
    {
        internal SagaCommandEndpoint()
        {
                
        }
        public TData Data { get; set; }
        
    }

    public class CommandEndpointBuilder<TData> : SagaCommandEndpoint where TData: class, ISagaCommand
    {
        private string _channel;
        private Type _replyData;
        private TData _data;

        public CommandEndpointBuilder(TData data)
        {
            _data = data;
        }

        public static CommandEndpointBuilder<TData> BuildCommandFor(TData data)
        {
            return new CommandEndpointBuilder<TData>(data);
        }

        public CommandEndpointBuilder<TData> CommandChannel(string channel)
        {
            _channel = channel;
            return this;
        }

        public CommandEndpointBuilder<TData> ReplyWithData(Type dataType)
        {
            _replyData = dataType;
            return this;
        }
        
        public SagaCommandEndpoint<TData> Build()
        {
            return new SagaCommandEndpoint<TData>()
            {
                Channel = this._channel,
                Data = this._data,
                ReplyDataType = this._replyData
            };
        }
        
    }
}