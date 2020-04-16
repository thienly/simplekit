using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using SimpleKit.StateMachine.Definitions;

namespace SagaContract
{
    public class SagaCommandContextContract
    {
        private string _destinationAddress;
        private string _faultAddress;
        public Guid MessageId { get; set; } // unique identifier for each message
        public Guid CorrelationId { get; set; } 
        public Guid RequestId { get; set; } // auto assign when starting a request, a respond message will auto attach it
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

        public  Dictionary<string, object> ToDictionary()
        {
            var data = new Dictionary<string,object>();
            var propertyInfos = typeof(SagaCommandContextContract).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var propertyInfo in propertyInfos)
            {
                try
                {
                    var value = propertyInfo.GetValue(this,null);
                    data.Add(propertyInfo.Name,value?.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                
            }

            return data;
        }
    }
}