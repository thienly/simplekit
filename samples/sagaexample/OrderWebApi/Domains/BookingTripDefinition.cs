using System;
using System.Text;
using Newtonsoft.Json;
using SimpleKit.StateMachine.Definitions;

namespace OrderWebApi.Domains
{
   

  

    public class BookingTripDefinition : SagaDefinition<BookingTripState>
    {
        private ISagaStepDefinition _definition;
        private TripManagementEndpoints _managementEndpoints;
        private BookingTripState _bookingTripState;

        public BookingTripDefinition(BookingTripState bookingTripState)
        {
            _managementEndpoints = new TripManagementEndpoints();
            _bookingTripState = bookingTripState;
            _definition = this.Step("BeginBooking")
                .AssignCompensation((cm,state) => _managementEndpoints.CancelHotel(cm, (BookingTripState)state),
                    _bookingTripState.CancelHotel)
                .Step("BookingHotel")
                .AssignParticipant((command,state) => _managementEndpoints.BookHotel(command, (BookingTripState)state),
                    _bookingTripState.BookHotel)
                .AssignCompensation((command,state) => _managementEndpoints.CancelCar(command, (BookingTripState)state),
                    _bookingTripState.CancelCar)
                .Step("BookCar")
                .AssignParticipant((command,state) => _managementEndpoints.BookCar(command, (BookingTripState)state),
                    _bookingTripState.BookCar)
                .Build();
        }

        public override SagaStepDefinition<BookingTripState> GetStepDefinition()
        {
            return (SagaStepDefinition<BookingTripState>) _definition;
        }
    }
}