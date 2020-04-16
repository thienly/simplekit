using System.Text;
using Newtonsoft.Json;
using SimpleKit.StateMachine.Definitions;

namespace SagaContract
{
    public class CancelHotelSagaCommandEndpoint : SagaCommandEndpoint
    {
        public CancelHotelSagaCommandEndpoint(CancelHotelMsg msg)
        {
            Data = Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(msg));
        }
    }
}