using Microsoft.EntityFrameworkCore;
using PuebloBonitoApi.Databases;
using PuebloBonitoApi.Domain.Reservations.Dtos;

namespace PuebloBonitoApi.Domain.Reservations.Features
{
    public static class GetReservationList
    {
        public static async Task<List<ReservationDto>> ExecuteAsync(PuebloBonitoDbContext context)
        {
            var reservations = await context.Reservations.Include(x => x.Guest).Include(x => x.HotelRoom).ToListAsync();

            return reservations.Select(reservation => new ReservationDto
            {
                Id = reservation.Id,
                HotelRoomId = reservation.HotelRoomId,
                GuestId = reservation.GuestId,
                ArrivalDate = reservation.ArrivalDate,
                DepartureDate = reservation.DepartureDate,
                TotalAdults = reservation.TotalAdults,
                TotalChildren = reservation.TotalChildren,
                Status = reservation.Status,
                AllInclusive = reservation.AllInclusive,
                Guest = reservation.Guest,
                HotelRoom = reservation.HotelRoom
            }).ToList();
        }
    }
}
