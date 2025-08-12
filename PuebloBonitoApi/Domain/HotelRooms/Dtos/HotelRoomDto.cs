using PuebloBonitoApi.Domain.Hotels.Dtos;
using PuebloBonitoApi.Domain.Rooms.Dtos;

namespace PuebloBonitoApi.Domain.HotelRooms.Dtos
{
    public class HotelRoomDto
    {
        public Guid Id { get; set; }
        public Guid HotelId { get; set; }
        public Guid RoomId { get; set; }
        public bool HasSeaView { get; set; }
        public int Number { get; set; }
        public int Floor { get; set; }
        public decimal Cost { get; set; }
        public bool Status { get; set; }

        public HotelDto? Hotel { get; set; }
        public RoomDto? Room { get; set; }
    }
}
