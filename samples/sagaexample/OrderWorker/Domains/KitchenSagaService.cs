using System.Collections.Generic;
using SimpleKit.StateMachine.Definitions;

namespace OrderWorker.Domains
{
    public class KitchenSagaService: ISagaCommandEndpointBuilder
    {
        public SagaCommandEndpoint AskKitchen(ISagaCommand command)
        {
            return CommandEndpointBuilder<AskingKitchenCommand>
                .BuildCommandFor((AskingKitchenCommand) command)
                .CommandChannel("Order_Kitchen")
                .ReplyWithData(typeof(KitchenReplyCommand))
                .Build();
        }

        public IReadOnlyCollection<string> GetReplyChannel()
        {
            return new[] {"KitchenReply"};
        }
    }
}