using Microsoft.EntityFrameworkCore.Storage;
using PuebloBonitoApi.Databases;
using PuebloBonitoApi.Domain.Guests.Dtos;
using PuebloBonitoApi.Domain.Hotels;
using PuebloBonitoApi.Exceptions;

namespace PuebloBonitoApi.Domain.Guests.Features
{
    public static class AddGuest
    {
        public static void Execute(PuebloBonitoDbContext dbContext, GuestForCreationDto guestForCreationDto)
        {
            using (IDbContextTransaction transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var guest = new Guest
                    {
                        Name = guestForCreationDto.Name,
                        LastName = guestForCreationDto.LastName,
                        Email = guestForCreationDto.Email,
                        Phone = guestForCreationDto.Phone,
                        BirthDate = guestForCreationDto.BirthDate,
                        Address = guestForCreationDto.Address
                    };

                    guest.UpdateCreationInfo(DateTimeOffset.Now);
                    guest.UpdateModificationInfo(DateTimeOffset.Now);

                    dbContext.Guests.Add(guest);
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
