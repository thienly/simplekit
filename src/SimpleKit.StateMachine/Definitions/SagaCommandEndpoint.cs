using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading;

namespace SimpleKit.StateMachine.Definitions
{
    public class SagaCommandContext
    {
        private string _destinationAddress;
        private string _faultAddress;
        public Guid MessageId { get; set; } // unique identifier for each message
        public Guid CorrelationId { get; set; }

        public Guid
            RequestId { get; set; } // auto assign when starting a request, a respond message will auto attach it

        public string SourceAddress { get; set; } // originated address

        public string DestinationAddress
        {
            get => _destinationAddress;
            set => _destinationAddress = value;
        } // Destination address

        public string ReplyAddress { get; set; }

        public string FaultAddress
        {
            get
            {
                if (_faultAddress != null)
                    return _faultAddress;
                return "Fault_" + _destinationAddress;
            }
            set => _faultAddress = value;
        }

        public TimeSpan ExpirationTime { get; set; } = Timeout.InfiniteTimeSpan;
        public string Host { get; set; }
        public string MessageType { get; set; }
        public string ReplyMessageType { get; set; }
        public string FaultMessageType { get; set; }

        public Dictionary<string, object> ToDictionary()
        {
            var data = new Dictionary<string, object>();
            var propertyInfos = typeof(SagaCommandContext).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var propertyInfo in propertyInfos)
            {
                var value = propertyInfo.GetValue(this);
                data.Add(propertyInfo.Name, value?.ToString());
            }

            return data;
        }
    }

    public abstract class SagaCommandEndpoint
    {
        public SagaCommandContext SagaCommandContext { get; set; }
        public byte[] Data { get; set; }
    }


    public class NoReplyCommandEndpoint : SagaCommandEndpoint
    {
    }

    public class WaitingReplyCommandEndpoint : SagaCommandEndpoint
    {
    }
}