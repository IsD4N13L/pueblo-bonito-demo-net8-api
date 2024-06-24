using Microsoft.EntityFrameworkCore.Storage;
using PuebloBonitoApi.Databases;
using PuebloBonitoApi.Domain.Reservations.Dtos;
using PuebloBonitoApi.Exceptions;

namespace PuebloBonitoApi.Domain.Reservations.Features
{
    public static class AddReservation
    {
        public static void Execute(PuebloBonitoDbContext dbContext, ReservationForCreationDto reservationForCreationDto)
        {
            using (IDbContextTransaction transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var reservation = new Reservation
                    {
                        HotelRoomId = reservationForCreationDto.HotelRoomId,
                        GuestId = reservationForCreationDto.GuestId,
                        ArrivalDate = reservationForCreationDto.ArrivalDate,
                        DepartureDate = reservationForCreationDto.DepartureDate,
                        TotalAdults = reservationForCreationDto.TotalAdults,
                        TotalChildren = reservationForCreationDto.TotalChildren,
                        Status = reservationForCreationDto.Status,
                        AllInclusive = reservationForCreationDto.AllInclusive
                    };
                    reservation.UpdateCreationInfo(DateTimeOffset.Now);
                    reservation.UpdateModificationInfo(DateTimeOffset.Now);

                    dbContext.Reservations.Add(reservation);
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
