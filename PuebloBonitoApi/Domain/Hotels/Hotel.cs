using PuebloBonitoApi.Domain.HotelRooms;

namespace PuebloBonitoApi.Domain.Hotels
{
    public class Hotel : BaseEntity
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public string Address { get; set; }

        private List<HotelRoom> _hotelRooms = new();
        public IReadOnlyCollection<HotelRoom> HotelRooms => _hotelRooms.AsReadOnly();

    }
}
