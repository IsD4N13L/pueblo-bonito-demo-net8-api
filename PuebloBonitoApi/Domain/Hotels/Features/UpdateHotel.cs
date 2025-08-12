using Microsoft.EntityFrameworkCore.Storage;
using PuebloBonitoApi.Databases;
using PuebloBonitoApi.Domain.Hotels.Dtos;
using PuebloBonitoApi.Exceptions;

namespace PuebloBonitoApi.Domain.Hotels.Features
{
    public static class UpdateHotel
    {
        public static async Task<HotelDto> ExecuteAsync(PuebloBonitoDbContext dbContext, Guid id, HotelForUpdateDto hotelForUpdateDto)
        {
            using (IDbContextTransaction transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var hotel = await dbContext.Hotels.FindAsync(id);
                    if (hotel == null)
                    {
                        throw new Exception("No se encontró e hotel");
                    }

                    hotel.Name = hotelForUpdateDto.Name;
                    hotel.Location = hotelForUpdateDto.Location;
                    hotel.Address = hotelForUpdateDto.Address;
                    hotel.UpdateModificationInfo(DateTimeOffset.Now);

                    dbContext.Hotels.Update(hotel);
                    await dbContext.SaveChangesAsync();

                    transaction.Commit();

                    return new HotelDto
                    {
                        Id = hotel.Id,
                        Name = hotel.Name,
                        Location = hotel.Location,
                        Address = hotel.Address
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
