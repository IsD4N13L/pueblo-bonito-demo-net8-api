using Microsoft.EntityFrameworkCore.Storage;
using PuebloBonitoApi.Databases;
using PuebloBonitoApi.Exceptions;

namespace PuebloBonitoApi.Domain.Hotels.Features
{
    public static class DeleteHotel
    {
        public static void Execute(PuebloBonitoDbContext dbContext, Guid id)
        {
            using (IDbContextTransaction transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var hotel = dbContext.Hotels.Find(id);
                    if (hotel == null)
                    {
                        throw new NotFoundException("No se encontró el hotel");
                    }
                    dbContext.Hotels.Remove(hotel);
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
