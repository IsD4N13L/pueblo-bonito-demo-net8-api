using Microsoft.EntityFrameworkCore.Storage;
using PuebloBonitoApi.Databases;
using PuebloBonitoApi.Exceptions;

namespace PuebloBonitoApi.Domain.Reservations.Features
{
    public static class DeleteReservation
    {
        public static void Execute(PuebloBonitoDbContext dbContext, Guid id)
        {
            using (IDbContextTransaction transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var reservation = dbContext.Reservations.Find(id);
                    if (reservation == null)
                    {
                        throw new NotFoundException("No se encontró la reservación");
                    }
                    dbContext.Reservations.Remove(reservation);
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
