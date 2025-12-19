using Applications.Interfaces.IRepository;
using Domains.Entities;
using Infrastructures.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class BookingRepository : IBookingRepository
    {
        private readonly AppDbContext _db;

        public BookingRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Booking> CreateBookingAsync(Booking booking)
        {
            _db.Bookings.Add(booking);
            await _db.SaveChangesAsync();
            return booking;
        }

        public async Task<IEnumerable<Booking>> GetAllBookingAsync()
        {
            return await _db.Bookings
                .Include(b => b.Event)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();
        }

        public async Task<Booking?> GetBookingByIdAsync(int id)
        {
            return await _db.Bookings
                .Include(b => b.Event)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<Booking>> GetBookingsByUserAsync(int userId)
        {
            return await _db.Bookings
                .Include(b => b.Event)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();
        }

        public async Task UpdateAsync(Booking booking)
        {
            _db.Bookings.Update(booking);
            await _db.SaveChangesAsync();
        }
    }
}
