using Microsoft.AspNetCore.Mvc;
using PuebloBonitoApi.Databases;
using PuebloBonitoApi.Domain.Rooms.Dtos;
using PuebloBonitoApi.Domain.Rooms.Features;

namespace PuebloBonitoApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RoomsController : Controller
    {
        private readonly PuebloBonitoDbContext puebloBonitoDbContext;

        public RoomsController(PuebloBonitoDbContext puebloBonitoDbContext)
        {
            this.puebloBonitoDbContext = puebloBonitoDbContext;
        }


        [HttpPost(Name = "AddRoom")]
        public async Task<IActionResult> AddRoom([FromBody] RoomForCreationDto roomForCreation)
        {
            Domain.Rooms.Features.AddRoom.Execute(this.puebloBonitoDbContext, roomForCreation);

            return Ok();
        }

        [HttpGet(Name = "GetRooms")]
        public async Task<IActionResult> GetRooms()
        {
            var rooms = await GetRoomList.ExecuteAsync(this.puebloBonitoDbContext);

            return Ok(rooms);
        }

        [HttpPut("{id}", Name = "UpdateRoom")]
        public async Task<IActionResult> UpdateRoom(Guid id, RoomForUpdateDto roomForUpdate)
        {
            var room = await Domain.Rooms.Features.UpdateRoom.ExecuteAsync(this.puebloBonitoDbContext, id, roomForUpdate);

            return Ok(room);
        }

        [HttpDelete("{id}", Name = "DeleteRoom")]
        public async Task<IActionResult> DeleteRoom(Guid id)
        {
            Domain.Rooms.Features.DeleteRoom.Execute(this.puebloBonitoDbContext, id);

            return Ok();
        }
    }
}
