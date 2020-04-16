using System.Text;
using Newtonsoft.Json;
using SimpleKit.StateMachine.Definitions;

namespace SagaContract
{
    public class BookHotelSagaCommandEndpoint : SagaCommandEndpoint
    {
        public BookHotelSagaCommandEndpoint(BookHotelMsg msg)
        {
            Data = Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(msg));
        }
    }
}