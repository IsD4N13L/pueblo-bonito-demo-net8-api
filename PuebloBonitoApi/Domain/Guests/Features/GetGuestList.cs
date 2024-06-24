using Microsoft.EntityFrameworkCore;
using PuebloBonitoApi.Databases;
using PuebloBonitoApi.Domain.Guests.Dtos;

namespace PuebloBonitoApi.Domain.Guests.Features
{
    public static class GetGuestList
    {
        public static async Task<List<GuestDto>> ExecuteAsync(PuebloBonitoDbContext context)
        {
            var guests = await context.Guests.ToListAsync();

            return guests.Select(guest => new GuestDto
            {
                Id = guest.Id,
                Name = guest.Name,
                LastName = guest.LastName,
                Email = guest.Email,
                Phone = guest.Phone,
                BirthDate = guest.BirthDate,
                Address = guest.Address
            }).ToList();
        }
    }
}
