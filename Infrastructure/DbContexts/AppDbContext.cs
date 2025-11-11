using Domains.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructures.DbContexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
            Console.WriteLine($"Connected to Database: {Database.GetDbConnection().Database}");
        }

        public DbSet<Event> Events { get; set; }
        public DbSet<Venue> Venues { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<AdminUser> Users { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<State> States { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //  Booking → Seat (one-to-many)
            modelBuilder.Entity<Booking>()
                .HasMany(b => b.Seats)
                .WithOne(s => s.Booking)
                .HasForeignKey(s => s.BookingId)
                .OnDelete(DeleteBehavior.NoAction);

            //  Country configuration
            modelBuilder.Entity<Country>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(200);
                b.Property(x => x.IsoCode)
                    .IsRequired()
                    .HasMaxLength(10);
                b.Property(x => x.CreatedAt)
                    .HasDefaultValueSql("SYSUTCDATETIME()");
            });

            // Country → State (1-to-many)
            modelBuilder.Entity<Country>()
                .HasMany(c => c.States)
                .WithOne(s => s.Country)
                .HasForeignKey(s => s.CountryId)
                .OnDelete(DeleteBehavior.Cascade);

            //  State → City (1-to-many)
            modelBuilder.Entity<State>()
                .HasMany(s => s.Cities)
                .WithOne(c => c.State)
                .HasForeignKey(c => c.StateId)
                .OnDelete(DeleteBehavior.Cascade);

            //  City → Venue (1-to-many)
            modelBuilder.Entity<City>()
                .HasMany(c => c.Venues)
                .WithOne(v => v.City)
                .HasForeignKey(v => v.CityId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
