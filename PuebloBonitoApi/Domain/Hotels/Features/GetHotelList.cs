using Microsoft.EntityFrameworkCore;
using PuebloBonitoApi.Databases;
using PuebloBonitoApi.Domain.HotelRooms.Dtos;
using PuebloBonitoApi.Domain.Hotels.Dtos;
using PuebloBonitoApi.Domain.Rooms.Dtos;

namespace PuebloBonitoApi.Domain.Hotels.Features
{
    public static class GetHotelList
    {
        public static async Task<List<HotelDto>> ExecuteAsync(PuebloBonitoDbContext context)
        {
            var hotels = await context.Hotels.Include(d => d.HotelRooms).AsNoTracking().ToListAsync();

            return hotels.Select(hotel => new HotelDto
            {
                Id = hotel.Id,
                Name = hotel.Name,
                Address = hotel.Address,
                Location = hotel.Location,
                HotelRooms = hotel.HotelRooms.Select(hr => new HotelRoomDto
                {
                    RoomId = hr.RoomId,
                    HotelId = hr.HotelId,
                    Id = hr.Id,
                    Cost = hr.Cost,
                    Floor = hr.Floor,
                    HasSeaView = hr.HasSeaView,
                    Number = hr.Number,
                    Status = hr.Status,
                    Room = context.Rooms.FirstOrDefault(r => r.Id == hr.RoomId)?.ToDto()
                }).ToList()
            }).ToList();
        }
    }
}
