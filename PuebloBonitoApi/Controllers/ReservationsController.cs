using Microsoft.AspNetCore.Mvc;
using PuebloBonitoApi.Databases;
using PuebloBonitoApi.Domain.Reservations.Dtos;
using PuebloBonitoApi.Domain.Reservations.Features;

namespace PuebloBonitoApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReservationsController : Controller
    {

        private readonly PuebloBonitoDbContext puebloBonitoDbContext;

        public ReservationsController(PuebloBonitoDbContext puebloBonitoDbContext)
        {
            this.puebloBonitoDbContext = puebloBonitoDbContext;
        }

        [HttpPost(Name = "AddReservation")]
        public async Task<IActionResult> AddReservation([FromBody] ReservationForCreationDto reservationForCreation)
        {
            Domain.Reservations.Features.AddReservation.Execute(this.puebloBonitoDbContext, reservationForCreation);

            return Ok();
        }

        [HttpGet(Name = "GetReservations")]
        public async Task<IActionResult> GetReservations()
        {
            var reservations = await GetReservationList.ExecuteAsync(this.puebloBonitoDbContext);

            return Ok(reservations);
        }

        [HttpPut("{id}", Name = "UpdateReservation")]
        public async Task<IActionResult> UpdateReservation(Guid id, ReservationForUpdateDto reservationForUpdate)
        {
            var reservation = await Domain.Reservations.Features.UpdateReservation.ExecuteAsync(this.puebloBonitoDbContext, id, reservationForUpdate);

            return Ok(reservation);
        }

        [HttpDelete("{id}", Name = "DeleteReservation")]
        public async Task<IActionResult> DeleteReservation(Guid id)
        {
            Domain.Reservations.Features.DeleteReservation.Execute(this.puebloBonitoDbContext, id);

            return Ok();
        }
    }
}
