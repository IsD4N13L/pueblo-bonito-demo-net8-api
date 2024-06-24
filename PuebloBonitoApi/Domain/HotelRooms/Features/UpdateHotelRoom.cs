using Microsoft.EntityFrameworkCore.Storage;
using PuebloBonitoApi.Databases;
using PuebloBonitoApi.Domain.HotelRooms.Dtos;
using PuebloBonitoApi.Exceptions;

namespace PuebloBonitoApi.Domain.HotelRooms.Features
{
    public static class UpdateHotelRoom
    {
        public static async Task<HotelRoomDto> ExecuteAsync(PuebloBonitoDbContext dbContext, Guid id, HotelRoomForUpdateDto hotelRoomForUpdateDto)
        {
            using (IDbContextTransaction transaction = dbContext.Database.BeginTransaction())
            {
                try
                {
                    var hotelRoom = await dbContext.HotelRooms.FindAsync(id);
                    if (hotelRoom == null)
                    {
                        throw new Exception("No se encontró la habitación");
                    }

                    hotelRoom.HasSeaView = hotelRoomForUpdateDto.HasSeaView;
                    hotelRoom.Number = hotelRoomForUpdateDto.Number;
                    hotelRoom.Floor = hotelRoomForUpdateDto.Floor;
                    hotelRoom.Cost = hotelRoomForUpdateDto.Cost;
                    hotelRoom.Status = hotelRoomForUpdateDto.Status;
                    hotelRoom.UpdateModificationInfo(DateTimeOffset.Now);

                    dbContext.HotelRooms.Update(hotelRoom);
                    await dbContext.SaveChangesAsync();

                    transaction.Commit();

                    return new HotelRoomDto
                    {
                        Id = hotelRoom.Id,
                        HotelId = hotelRoom.HotelId,
                        RoomId = hotelRoom.RoomId,
                        HasSeaView = hotelRoom.HasSeaView,
                        Number = hotelRoom.Number,
                        Floor = hotelRoom.Floor,
                        Cost = hotelRoom.Cost,
                        Status = hotelRoom.Status
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
