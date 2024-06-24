using Microsoft.EntityFrameworkCore.Storage;
using PuebloBonitoApi.Databases;
using PuebloBonitoApi.Exceptions;

namespace PuebloBonitoApi.Domain.Rooms.Features
{
    public static class DeleteRoom
    {
        public static void Execute(PuebloBonitoDbContext dbContext, Guid id)
        {
            using (IDbContextTransaction transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var room = dbContext.Rooms.Find(id);
                    if (room == null)
                    {
                        throw new NotFoundException("No se encontró la habitación");
                    }
                    dbContext.Rooms.Remove(room);
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
