using PuebloBonitoApi.Domain.Hotels;
using PuebloBonitoApi.Domain.Reservations;
using PuebloBonitoApi.Domain.Rooms;
using System.ComponentModel.DataAnnotations.Schema;

namespace PuebloBonitoApi.Domain.HotelRooms
{
    public class HotelRoom : BaseEntity
    {
        [Column("hotel_id")]
        public Guid HotelId { get; set; }
        [Column("room_id")]
        public Guid RoomId { get; set; }
        [Column("has_sea_view")]
        public bool HasSeaView { get; set; }
        public int Number { get; set; }
        public int Floor { get; set; }
        public decimal Cost { get; set; }
        public bool Status { get; set; }
        public Hotel Hotel { get; }
        public Room Room { get; }

        private readonly List<Reservation> _reservations = new();
        public IReadOnlyCollection<Reservation> Reservations => _reservations;
    }
}
