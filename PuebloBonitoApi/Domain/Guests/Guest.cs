using PuebloBonitoApi.Domain.Reservations;

namespace PuebloBonitoApi.Domain.Guests
{
    public class Guest : BaseEntity
    {

        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateOnly BirthDate { get; set; }
        public string Address { get; set; }

        public ICollection<Reservation> Reservations { get; set; }
    }
}
