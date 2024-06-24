namespace PuebloBonitoApi.Domain.HotelRooms.Dtos
{
    public class HotelRoomForUpdateDto
    {
        public Guid HotelId { get; set; }
        public Guid RoomId { get; set; }
        public bool HasSeaView { get; set; }
        public int Number { get; set; }
        public int Floor { get; set; }
        public decimal Cost { get; set; }
        public bool Status { get; set; }
    }
}
