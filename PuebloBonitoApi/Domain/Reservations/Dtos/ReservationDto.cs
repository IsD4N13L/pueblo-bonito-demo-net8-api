using PuebloBonitoApi.Domain.Guests;
using PuebloBonitoApi.Domain.HotelRooms;

namespace PuebloBonitoApi.Domain.Reservations.Dtos
{
    public class ReservationDto
    {
        public Guid Id { get; set; }
        public Guid HotelRoomId { get; set; }
        public Guid GuestId { get; set; }
        public DateTime ArrivalDate { get; set; }
        public DateTime DepartureDate { get; set; }
        public int TotalAdults { get; set; }
        public int TotalChildren { get; set; }
        public bool Status { get; set; }
        public bool AllInclusive { get; set; }

        public Guest Guest { get; set; }
        public HotelRoom HotelRoom { get; set; }
    }
}
