using PuebloBonitoApi.Domain.HotelRooms;
using PuebloBonitoApi.Domain.Rooms.Dtos;

namespace PuebloBonitoApi.Domain.Rooms
{
    public class Room : BaseEntity
    {
        public string Name { get; set; }
        public string Type { get; set; }

        public ICollection<HotelRoom> HotelRooms { get; set; }

        public RoomDto ToDto()
        {
            return new RoomDto
            {
                Id = this.Id,
                Name = this.Name,
                Type = this.Type
            };
        }
    }
}
