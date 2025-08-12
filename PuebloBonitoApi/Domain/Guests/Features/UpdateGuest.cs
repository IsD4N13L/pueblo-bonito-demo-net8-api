using Microsoft.EntityFrameworkCore.Storage;
using PuebloBonitoApi.Databases;
using PuebloBonitoApi.Domain.Guests.Dtos;
using PuebloBonitoApi.Exceptions;

namespace PuebloBonitoApi.Domain.Guests.Features
{
    public static class UpdateGuest
    {
        public static async Task<GuestDto> ExecuteAsync(PuebloBonitoDbContext dbContext, Guid id, GuestForUpdateDto guestForUpdateDto)
        {
            using (IDbContextTransaction transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var guest = await dbContext.Guests.FindAsync(id);
                    if (guest == null)
                    {
                        throw new NotFoundException("No se encontró el huésped");
                    }

                    guest.Name = guestForUpdateDto.Name;
                    guest.LastName = guestForUpdateDto.LastName;
                    guest.Email = guestForUpdateDto.Email;
                    guest.Phone = guestForUpdateDto.Phone;
                    guest.BirthDate = guestForUpdateDto.BirthDate;
                    guest.Address = guestForUpdateDto.Address;
                    guest.UpdateModificationInfo(DateTimeOffset.Now);

                    dbContext.Guests.Update(guest);
                    await dbContext.SaveChangesAsync();
                    transaction.Commit();

                    return new GuestDto
                    {
                        Id = guest.Id,
                        Name = guest.Name,
                        LastName = guest.LastName,
                        Email = guest.Email,
                        Phone = guest.Phone,
                        BirthDate = guest.BirthDate,
                        Address = guest.Address
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
