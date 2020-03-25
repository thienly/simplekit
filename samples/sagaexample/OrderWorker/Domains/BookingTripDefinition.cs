using SimpleKit.StateMachine.Definitions;

namespace OrderWorker.Domains
{
    public class BookingTripDefinition : SagaDefinition<BookingTripState>
    {
        private ISagaStepDefinition _definition;
        private BookingCommandHandler _bookingCommandHandler;
        private BookingTripState _bookingTripState;

        public BookingTripDefinition(BookingTripState bookingTripState)
        {
            _bookingCommandHandler = new BookingCommandHandler();
            _bookingTripState = bookingTripState;
            _definition = this.Step("BeginBooking")
                .AssignCompensation((cm,state) => _bookingCommandHandler.CancelHotel(cm, (BookingTripState)state),
                    _bookingTripState.CancelHotel)
                .Step("BookingHotel")
                .AssignParticipant((command,state) => _bookingCommandHandler.BookHotel(command, (BookingTripState)state),
                    _bookingTripState.BookHotel)
                .AssignCompensation((command,state) => _bookingCommandHandler.CancelCar(command, (BookingTripState)state),
                    _bookingTripState.CancelCar)
                .Step("BookCar")
                .AssignParticipant((command,state) => _bookingCommandHandler.BookCar(command, (BookingTripState)state),
                    _bookingTripState.BookCar)
                .Build();
        }

        public override SagaStepDefinition<BookingTripState> GetStepDefinition()
        {
            return (SagaStepDefinition<BookingTripState>) _definition;
        }
    }
}