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
            //_db.Attach(booking.Event);

            var seatIds = booking.Seats.Select(s => s.Id).ToList();

            var seatsToBook = await _db.Seats
                .Where(s => seatIds
                .Contains(s.Id))
                .ToListAsync();

            // Validate no seat is already booked
            var bookedSeats = seatsToBook.Where(s => s.IsBooked).Select(s => s.SeatNumber).ToList();
            if (bookedSeats.Any())
            {
                var seatList = string.Join(", ", bookedSeats);
                throw new Exception($"The following seats are already booked: {seatList}. Please choose different seats.");
            }

            foreach (var seat in booking.Seats)
            {
                seat.IsBooked = true;
                seat.Booking = booking;
            }

            _db.Bookings.Add(booking);

            await _db.SaveChangesAsync();
            return booking;
        }


        public async Task<IEnumerable<Booking>> GetAllBookingAsync()
        {
            return await _db.Bookings
                .Include(b => b.Event)
                .Include(b => b.Seats)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();
        }

        public async Task<Booking?> GetBookingByIdAsync(int id)
        {
            return await _db.Bookings
                .Include(b => b.Event)
                .Include(b => b.Seats)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<Booking>> GetBookingsByUserAsync(string userEmail)
        {
            return await _db.Bookings
                .Include(b => b.Event)
                .Include(b => b.Seats)
                .Where(b => b.UserEmail == userEmail)
                .ToListAsync();

        }

        public async Task<bool> DeleteBookingAsync(int id)
        {
            var booking = await _db.Bookings
                .Include(b => b.Event)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
                return false;

            foreach (var seat in booking.Seats)
            {
                seat.IsBooked = false;
            }

            _db.Bookings.Remove(booking);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task ReleaseSeatsAsync(int bookingId)
        {
            var booking = await _db.Bookings
                .Include(b => b.Seats)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null)
                throw new Exception($"Booking with Id {bookingId} not found .");

            foreach (var seat in booking.Seats)
            {
                seat.IsBooked = false;
                seat.BookingId = null;
            }
            _db.Bookings.Remove(booking);
            await _db.SaveChangesAsync();

        }

    }
}
