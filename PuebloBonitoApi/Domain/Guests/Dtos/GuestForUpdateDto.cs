namespace PuebloBonitoApi.Domain.Guests.Dtos
{
    public class GuestForUpdateDto
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; } 
        public string Phone { get; set; }
        public DateOnly BirthDate { get; set; }
        public string Address { get; set; }
    }
}
