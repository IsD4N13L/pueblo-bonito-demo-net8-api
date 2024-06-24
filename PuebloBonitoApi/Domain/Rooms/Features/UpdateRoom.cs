using Microsoft.EntityFrameworkCore.Storage;
using PuebloBonitoApi.Databases;
using PuebloBonitoApi.Domain.Rooms.Dtos;
using PuebloBonitoApi.Exceptions;

namespace PuebloBonitoApi.Domain.Rooms.Features
{
    public static class UpdateRoom
    {
        public static async Task<RoomDto> ExecuteAsync(PuebloBonitoDbContext dbContext, Guid id, RoomForUpdateDto roomForUpdateDto)
        {
            using (IDbContextTransaction transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var room = await dbContext.Rooms.FindAsync(id);
                    if (room == null)
                    {
                        throw new NotFoundException("No se encontró la habitación");
                    }

                    room.Name = roomForUpdateDto.Name;
                    room.Type = roomForUpdateDto.Type;
                    room.UpdateModificationInfo(DateTimeOffset.Now);

                    dbContext.Rooms.Update(room);
                    await dbContext.SaveChangesAsync();
                    transaction.Commit();

                    return new RoomDto
                    {
                        Id = room.Id,
                        Name = room.Name,
                        Type = room.Type
                    };
                }
                catch
                {
                    transaction.Rollback();
                    throw new InternalServerErrorException();
                }
            }

        }
    }
}
