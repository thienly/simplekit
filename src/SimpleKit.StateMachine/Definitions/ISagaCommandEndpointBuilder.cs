using System.Collections.Generic;

namespace SimpleKit.StateMachine.Definitions
{
    public interface ISagaCommandEndpointBuilder
    {
        IReadOnlyCollection<string> GetReplyChannel();
    }
}