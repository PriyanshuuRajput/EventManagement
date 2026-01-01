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
        //public DbSet<Seat> Seats { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<AdminUser> Users { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<EventCategory> EventCategories { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Role>()
     .HasMany(r => r.Users)
     .WithOne(u => u.Role)
     .HasForeignKey(u => u.RoleId)
     .OnDelete(DeleteBehavior.Restrict);


            // AdminUser ⇄ Manager (One-to-One)
            // ===========================
            modelBuilder.Entity<AdminUser>()
                .HasOne(u => u.Manager)
                .WithOne(m => m.User)
                .HasForeignKey<Manager>(m => m.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Event ⇄ Manager (One-to-Many)
            modelBuilder.Entity<Event>()
                .HasOne(e => e.Managers)
                .WithMany(m => m.Events)
                .HasForeignKey(e => e.ManagerId)
                .HasPrincipalKey(m => m.Id)
                .OnDelete(DeleteBehavior.Restrict);


            //  3. Booking ⇄ Seat (One-to-Many)
            //// ===========================
            //modelBuilder.Entity<Booking>()
            //    //.HasMany(b => b.Seats)
            //    .WithOne(s => s.Booking)
            //    .HasForeignKey(s => s.BookingId)
            //    .OnDelete(DeleteBehavior.NoAction);

            //  Country config
            // ===========================
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

            // Country → State
            modelBuilder.Entity<Country>()
                .HasMany(c => c.States)
                .WithOne(s => s.Country)
                .HasForeignKey(s => s.CountryId)
                .OnDelete(DeleteBehavior.Cascade);

            // State → City
            modelBuilder.Entity<State>()
                .HasMany(s => s.Cities)
                .WithOne(c => c.State)
                .HasForeignKey(c => c.StateId)
                .OnDelete(DeleteBehavior.Cascade);

            // City → Venue
            modelBuilder.Entity<City>()
                .HasMany(c => c.Venues)
                .WithOne(v => v.City)
                .HasForeignKey(v => v.CityId)
                .OnDelete(DeleteBehavior.Cascade);
            // Venue → Events (One-to-Many)
            modelBuilder.Entity<Venue>()
                .HasMany(v => v.Events)
                .WithOne(e => e.Venue)
                .HasForeignKey(e => e.VenueId)
                .OnDelete(DeleteBehavior.Cascade);

            // EventCategory → Events (1:N)
            modelBuilder.Entity<EventCategory>()
                .HasMany(ec => ec.Events)
                .WithOne(e => e.EventCategory)
                .HasForeignKey(e => e.EventCategoryId)
                .OnDelete(DeleteBehavior.SetNull);


        }
    }
}
