
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PuebloBonitoApi.Domain.Guests;
using PuebloBonitoApi.Domain.HotelRooms;
using PuebloBonitoApi.Domain.Hotels;
using PuebloBonitoApi.Domain.Reservations;
using PuebloBonitoApi.Domain.Rooms;

namespace PuebloBonitoApi.Databases
{
    public sealed class PuebloBonitoDbContext(DbContextOptions<PuebloBonitoDbContext> options) : DbContext(options)
    {
        public DbSet<Guest> Guests { get; set; }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<HotelRoom> HotelRooms { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Guest>().HasKey(g => g.Id);
            modelBuilder.Entity<Hotel>().HasKey(h => h.Id);
            modelBuilder.Entity<Hotel>().HasMany(h => h.HotelRooms).WithOne(hr => hr.Hotel);
            modelBuilder.Entity<Room>().HasKey(r => r.Id);
            modelBuilder.Entity<HotelRoom>().HasOne(hr => hr.Room).WithMany(r => r.HotelRooms);
            modelBuilder.Entity<Reservation>().HasKey(r => r.Id);
            modelBuilder.Entity<Reservation>().HasOne(r => r.Guest).WithMany(g => g.Reservations);
            modelBuilder.Entity<Reservation>().HasOne(r => r.HotelRoom).WithMany(hr => hr.Reservations);
            
        }
    }
}
