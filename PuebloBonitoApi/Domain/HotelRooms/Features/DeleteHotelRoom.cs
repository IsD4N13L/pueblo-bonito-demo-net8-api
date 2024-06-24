using Microsoft.EntityFrameworkCore.Storage;
using PuebloBonitoApi.Databases;
using PuebloBonitoApi.Exceptions;

namespace PuebloBonitoApi.Domain.HotelRooms.Features
{
    public static class DeleteHotelRoom
    {
        public static void Execute(PuebloBonitoDbContext dbContext, Guid id)
        {
            using (IDbContextTransaction transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var hotelRoom = dbContext.HotelRooms.Find(id);
                    if (hotelRoom == null)
                    {
                        throw new NotFoundException("No se encontró la habitación");
                    }
                    dbContext.HotelRooms.Remove(hotelRoom);
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
