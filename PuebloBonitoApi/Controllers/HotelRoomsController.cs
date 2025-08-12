using Microsoft.AspNetCore.Mvc;
using PuebloBonitoApi.Databases;
using PuebloBonitoApi.Domain.HotelRooms.Dtos;
using PuebloBonitoApi.Domain.HotelRooms.Features;

namespace PuebloBonitoApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HotelRoomsController : Controller
    {
        private readonly PuebloBonitoDbContext puebloBonitoDbContext;

        public HotelRoomsController(PuebloBonitoDbContext puebloBonitoDbContext)
        {
            this.puebloBonitoDbContext = puebloBonitoDbContext;
        }

        [HttpPost(Name = "AddHotelRoom")]
        public async Task<IActionResult> AddHotelRoom([FromBody] HotelRoomForCreationDto hotelRoomForCreation)
        {
            Domain.HotelRooms.Features.AddHotelRoom.Execute(this.puebloBonitoDbContext, hotelRoomForCreation);

            return Ok();
        }

        [HttpGet(Name = "GetHotelRooms")]
        public async Task<IActionResult> GetHotelRooms()
        {
            var hotelRooms = await GetHotelRoomList.ExecuteAsync(this.puebloBonitoDbContext);

            return Ok(hotelRooms);
        }

        [HttpPut("{id}", Name = "UpdateHotelRoom")]
        public async Task<IActionResult> UpdateHotelRoom(Guid id, HotelRoomForUpdateDto hotelRoomForUpdate)
        {
            var hotelRoom = await Domain.HotelRooms.Features.UpdateHotelRoom.ExecuteAsync(this.puebloBonitoDbContext, id, hotelRoomForUpdate);

            return Ok(hotelRoom);
        }

        [HttpDelete("{id}", Name = "DeleteHotelRoom")]
        public async Task<IActionResult> DeleteHotelRoom(Guid id)
        {
            Domain.HotelRooms.Features.DeleteHotelRoom.Execute(this.puebloBonitoDbContext, id);

            return Ok();
        }
    }
}
