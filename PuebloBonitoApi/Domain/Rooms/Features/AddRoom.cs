using Microsoft.EntityFrameworkCore.Storage;
using PuebloBonitoApi.Databases;
using PuebloBonitoApi.Domain.Rooms.Dtos;
using PuebloBonitoApi.Exceptions;

namespace PuebloBonitoApi.Domain.Rooms.Features
{
    public static class AddRoom
    {
        public static void Execute(PuebloBonitoDbContext dbContext, RoomForCreationDto roomForCreationDto)
        {
            using (IDbContextTransaction transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var room = new Room
                    {
                        Name = roomForCreationDto.Name,
                        Type = roomForCreationDto.Type,
                    };

                    room.UpdateCreationInfo(DateTimeOffset.Now);
                    room.UpdateModificationInfo(DateTimeOffset.Now);

                    dbContext.Rooms.Add(room);
                    dbContext.SaveChanges();
                    transaction.Commit();
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
