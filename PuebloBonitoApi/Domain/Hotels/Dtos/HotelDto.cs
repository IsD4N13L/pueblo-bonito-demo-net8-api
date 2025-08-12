using PuebloBonitoApi.Domain.HotelRooms;
using PuebloBonitoApi.Domain.HotelRooms.Dtos;

namespace PuebloBonitoApi.Domain.Hotels.Dtos
{
    public class HotelDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Address { get; set; }

        public List<HotelRoomDto> HotelRooms { get; set; }
    }
}
