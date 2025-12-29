using Applications.Dto;
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
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<Booking?> GetBookingByIdAsync(int id)
        {
            return await _db.Bookings
                .Include(b => b.Event)
                    .ThenInclude(e => e.Venue)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<Booking>> GetBookingsByUserAsync(int userId)
        {
            return await _db.Bookings
                .Include(b => b.Event)
                    .ThenInclude(e => e.Venue)
                .Where(b => b.UserId == userId && b.Event != null)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }
        public async Task<int> GetActiveTicketCountByEventAsync(int eventId)
        {
            return await _db.Bookings
                .Where(b =>
                    b.EventId == eventId &&
                    b.PaymentStatus != PaymentStatus.Cancelled)
                .SumAsync(b => b.TicketCount);
        }

        public async Task<int> GetUserTicketCountByEventAsync(int eventId, int userId)
        {
            return await _db.Bookings
                .Where(b =>
                    b.EventId == eventId &&
                    b.UserId == userId &&
                    b.PaymentStatus != PaymentStatus.Cancelled)
                .SumAsync(b => b.TicketCount);
        }

        public async Task UpdateAsync(Booking booking)
        {
            _db.Bookings.Update(booking);
            await _db.SaveChangesAsync();
        }

        public async Task<AdminUser?> GetUserByIdAsync(int userId)
        {
            return await _db.Users
                .FirstOrDefaultAsync(b => b.Id == userId);
        }
        public async Task<Booking?> GetBookingByQrAsync(string qrCode)
        {
            return await _db.Bookings
                .Include(b => b.Event)
                    .ThenInclude(e => e.Venue)
                .FirstOrDefaultAsync(b => b.QrCode == qrCode);
        }


        public async Task<ManagerRevenueDto> GetEventStatsAsync(int eventId)
        {
            return await _db.Bookings
                .Where(b => b.EventId == eventId &&
                            b.PaymentStatus != PaymentStatus.Cancelled)
                .Select(b => new
                {
                    b.TicketCount,
                    TicketPrice = b.Event!.TicketPrice
                })
                .GroupBy(_ => 1)
                .Select(g => new ManagerRevenueDto
                {
                    TicketsSold = g.Sum(x => x.TicketCount),
                    Revenue = g.Sum(x => x.TicketCount * x.TicketPrice),
                    BookingCount = g.Count()
                })
                .FirstOrDefaultAsync()
                ?? new ManagerRevenueDto();
        }


    }
}
