using Microsoft.EntityFrameworkCore.Storage;
using PuebloBonitoApi.Databases;
using PuebloBonitoApi.Domain.HotelRooms.Dtos;
using PuebloBonitoApi.Exceptions;

namespace PuebloBonitoApi.Domain.HotelRooms.Features
{
    public static class AddHotelRoom
    {
        public static void Execute(PuebloBonitoDbContext dbContext, HotelRoomForCreationDto hotelRoomForCreationDto)
        {
            using (IDbContextTransaction transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var hotelRoom = new HotelRoom
                    {
                        HotelId = hotelRoomForCreationDto.HotelId,
                        RoomId = hotelRoomForCreationDto.RoomId,
                        HasSeaView = hotelRoomForCreationDto.HasSeaView,
                        Number = hotelRoomForCreationDto.Number,
                        Floor = hotelRoomForCreationDto.Floor,
                        Cost = hotelRoomForCreationDto.Cost,
                        Status = hotelRoomForCreationDto.Status
                    };
                    hotelRoom.UpdateCreationInfo(DateTimeOffset.Now);
                    hotelRoom.UpdateModificationInfo(DateTimeOffset.Now);

                    dbContext.HotelRooms.Add(hotelRoom);
                    dbContext.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new InternalServerErrorException(ex.Message);
                }
            }
        }
    }
}
