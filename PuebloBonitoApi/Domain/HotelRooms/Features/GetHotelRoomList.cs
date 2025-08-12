using Microsoft.EntityFrameworkCore;
using PuebloBonitoApi.Databases;
using PuebloBonitoApi.Domain.HotelRooms.Dtos;

namespace PuebloBonitoApi.Domain.HotelRooms.Features
{
    public static class GetHotelRoomList
    {
        public static async Task<List<HotelRoomDto>> ExecuteAsync(PuebloBonitoDbContext context)
        {
            var hotelRooms = await context.HotelRooms.ToListAsync();

            return hotelRooms.Select(hotelRoom => new HotelRoomDto
            {
                Id = hotelRoom.Id,
                HotelId = hotelRoom.HotelId,
                RoomId = hotelRoom.RoomId,
                HasSeaView = hotelRoom.HasSeaView,
                Number = hotelRoom.Number,
                Floor = hotelRoom.Floor,
                Cost = hotelRoom.Cost,
                Status = hotelRoom.Status
            }).ToList();
        }
    }
}
