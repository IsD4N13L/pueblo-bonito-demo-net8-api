using Microsoft.EntityFrameworkCore.Storage;
using PuebloBonitoApi.Databases;
using PuebloBonitoApi.Domain.Reservations.Dtos;
using PuebloBonitoApi.Exceptions;

namespace PuebloBonitoApi.Domain.Reservations.Features
{
    public static class UpdateReservation
    {
        public static async Task<ReservationDto> ExecuteAsync(PuebloBonitoDbContext dbContext, Guid id, ReservationForUpdateDto reservationForUpdateDto)
        {
            using (IDbContextTransaction transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var reservation = await dbContext.Reservations.FindAsync(id);
                    if (reservation == null)
                    {
                        throw new Exception("No se encontró la reservación");
                    }

                    reservation.ArrivalDate = reservationForUpdateDto.ArrivalDate;
                    reservation.DepartureDate = reservationForUpdateDto.DepartureDate;
                    reservation.TotalAdults = reservationForUpdateDto.TotalAdults;
                    reservation.TotalChildren = reservationForUpdateDto.TotalChildren;
                    reservation.Status = reservationForUpdateDto.Status;
                    reservation.AllInclusive = reservationForUpdateDto.AllInclusive;
                    reservation.UpdateModificationInfo(DateTimeOffset.Now);

                    dbContext.Reservations.Update(reservation);
                    await dbContext.SaveChangesAsync();

                    transaction.Commit();

                    return new ReservationDto
                    {
                        Id = reservation.Id,
                        HotelRoomId = reservation.HotelRoomId,
                        GuestId = reservation.GuestId,
                        ArrivalDate = reservation.ArrivalDate,
                        DepartureDate = reservation.DepartureDate,
                        TotalAdults = reservation.TotalAdults,
                        TotalChildren = reservation.TotalChildren,
                        Status = reservation.Status,
                        AllInclusive = reservation.AllInclusive
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
