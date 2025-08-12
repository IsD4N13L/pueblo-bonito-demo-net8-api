using Microsoft.EntityFrameworkCore;
using PuebloBonitoApi.Databases;
using PuebloBonitoApi.Domain.Rooms.Dtos;

namespace PuebloBonitoApi.Domain.Rooms.Features
{
    public static class GetRoomList
    {
        public static async Task<List<RoomDto>> ExecuteAsync(PuebloBonitoDbContext context)
        {
            var rooms = await context.Rooms.ToListAsync();

            return rooms.Select(room => new RoomDto
            {
                Id = room.Id,
                Name = room.Name,
                Type = room.Type,
            }).ToList();
        }
    }
}
