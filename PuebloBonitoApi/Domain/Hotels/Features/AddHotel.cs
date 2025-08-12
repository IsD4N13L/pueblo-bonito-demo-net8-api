using Microsoft.EntityFrameworkCore.Storage;
using PuebloBonitoApi.Databases;
using PuebloBonitoApi.Domain.Hotels.Dtos;
using PuebloBonitoApi.Exceptions;

namespace PuebloBonitoApi.Domain.Hotels.Features
{
    public static class AddHotel
    {
        public static void Execute(PuebloBonitoDbContext dbContext, HotelForCreationDto hotelForCreationDto)
        {
            using (IDbContextTransaction transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var hotel = new Hotel
                    {
                        Name = hotelForCreationDto.Name,
                        Address = hotelForCreationDto.Address,
                        Location = hotelForCreationDto.Location
                    };
                    hotel.UpdateCreationInfo(DateTimeOffset.Now);
                    hotel.UpdateModificationInfo(DateTimeOffset.Now);

                    dbContext.Hotels.Add(hotel);
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
