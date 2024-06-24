using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PuebloBonitoApi.Databases;
using PuebloBonitoApi.Domain.Hotels.Dtos;
using PuebloBonitoApi.Domain.Hotels.Features;

namespace PuebloBonitoApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HotelsController : ControllerBase
    {

        private readonly PuebloBonitoDbContext puebloBonitoDbContext;

        public HotelsController(PuebloBonitoDbContext puebloBonitoDbContext)
        {
            this.puebloBonitoDbContext = puebloBonitoDbContext;
        }

        [HttpPost(Name = "AddHotel")]
        public async Task<IActionResult> AddHotel([FromBody] HotelForCreationDto hotelForCreation)
        {
            Domain.Hotels.Features.AddHotel.Execute(this.puebloBonitoDbContext, hotelForCreation);

            return Ok();
        }

        [HttpGet(Name = "GetHotels")]
        public async Task<IActionResult> GetHotels()
        {
            var hotels = await GetHotelList.ExecuteAsync(this.puebloBonitoDbContext);

            return Ok(hotels);
        }

        [HttpPut("{id}", Name = "UpdateHotel")]
        public async Task<IActionResult> UpdateHotel(Guid id, HotelForUpdateDto hotelForUpdate)
        {
            var hotel = await Domain.Hotels.Features.UpdateHotel.ExecuteAsync(this.puebloBonitoDbContext, id, hotelForUpdate);

            return Ok(hotel);
        }

        [HttpDelete("{id}", Name = "DeleteHotel")]
        public async Task<IActionResult> DeleteHotel(Guid id)
        {
            Domain.Hotels.Features.DeleteHotel.Execute(this.puebloBonitoDbContext, id);

            return Ok();
        }
    }
}
