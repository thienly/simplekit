using System;

namespace OrderWebApi.Domains
{
    public class BookingTrip
    {
        public Guid CarId { get; private set; }
        public Guid HotelId { get; private set; }
        public string Status { get; private set; }

        public void BookCarWithId(Guid id)
        {
            CarId = id;
        }

        public void BookHotelWithId(Guid id)
        {
            HotelId = id;
        }

        public void SetStatus(string status)
        {
            Status += status;
        }
    }
}