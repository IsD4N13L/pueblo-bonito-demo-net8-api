using PuebloBonitoApi.Domain.Guests;
using PuebloBonitoApi.Domain.HotelRooms;
using System.ComponentModel.DataAnnotations.Schema;

namespace PuebloBonitoApi.Domain.Reservations
{
    public class Reservation : BaseEntity
    {
        [Column("hotel_room_id")]
        public Guid HotelRoomId { get; set; }
        [Column("guest_id")]
        public Guid GuestId { get; set; }
        [Column("arrival_date")]
        public DateTime ArrivalDate { get; set; }
        [Column("departure_date")]
        public DateTime DepartureDate { get; set; }
        [Column("total_adults")]
        public int TotalAdults { get; set; }
        [Column("total_children")]
        public int TotalChildren { get; set; }
        public bool Status { get; set; }
        [Column("all_inclusive")]
        public bool AllInclusive { get; set; }

        public HotelRoom HotelRoom { get; set; }
        public Guest Guest { get; set; }
    }
}
