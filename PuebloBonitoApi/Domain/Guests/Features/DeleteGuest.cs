using Microsoft.EntityFrameworkCore.Storage;
using PuebloBonitoApi.Databases;
using PuebloBonitoApi.Exceptions;

namespace PuebloBonitoApi.Domain.Guests.Features
{
    public static class DeleteGuest
    {
        public static void Execute(PuebloBonitoDbContext dbContext, Guid id)
        {
            using (IDbContextTransaction transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var guest = dbContext.Guests.Find(id);
                    if (guest == null)
                    {
                        throw new NotFoundException("No se encontró el huésped");
                    }
                    dbContext.Guests.Remove(guest);
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
