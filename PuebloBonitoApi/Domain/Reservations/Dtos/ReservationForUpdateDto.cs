namespace PuebloBonitoApi.Domain.Reservations.Dtos
{
    public class ReservationForUpdateDto
    {
        public Guid HotelRoomId { get; set; }
        public Guid GuestId { get; set; }
        public DateTime ArrivalDate { get; set; }
        public DateTime DepartureDate { get; set; }
        public int TotalAdults { get; set; }
        public int TotalChildren { get; set; }
        public bool Status { get; set; }
        public bool AllInclusive { get; set; }
    }
}
