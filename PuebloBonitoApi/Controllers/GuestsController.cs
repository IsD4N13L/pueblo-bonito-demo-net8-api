using Microsoft.AspNetCore.Mvc;
using PuebloBonitoApi.Databases;
using PuebloBonitoApi.Domain.Guests.Dtos;
using PuebloBonitoApi.Domain.Guests.Features;

namespace PuebloBonitoApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GuestsController : ControllerBase
    {
        private readonly PuebloBonitoDbContext puebloBonitoDbContext;

        public GuestsController(PuebloBonitoDbContext puebloBonitoDbContext)
        {
            this.puebloBonitoDbContext = puebloBonitoDbContext;
        }

        [HttpPost(Name = "AddGuest")]
        public async Task<IActionResult> AddGuest([FromBody] GuestForCreationDto guestForCreation)
        {
            Domain.Guests.Features.AddGuest.Execute(this.puebloBonitoDbContext, guestForCreation);

            return Ok();
        }

        [HttpGet(Name = "GetGuests")]
        public async Task<IActionResult> GetGuests()
        {
            var guests = await GetGuestList.ExecuteAsync(this.puebloBonitoDbContext);

            return Ok(guests);
        }

        [HttpPut("{id}", Name = "UpdateGuest")]
        public async Task<IActionResult> UpdateGuest(Guid id, GuestForUpdateDto guestForUpdate)
        {
            var guest = await Domain.Guests.Features.UpdateGuest.ExecuteAsync(this.puebloBonitoDbContext, id, guestForUpdate);

            return Ok(guest);
        }

        [HttpDelete("{id}", Name = "DeleteGuest")]
        public async Task<IActionResult> DeleteGuest(Guid id)
        {
            Domain.Guests.Features.DeleteGuest.Execute(this.puebloBonitoDbContext, id);

            return Ok();
        }
    }
}
