//using Applications.Interfaces.IRepository;
//using Domains.Entities;
//using Infrastructures.DbContexts;
//using Microsoft.EntityFrameworkCore;

//namespace Infrastructure.Repository
//{
//    public class SeatRepository : ISeatRepository
//    {
//        private readonly AppDbContext _db;
//        public SeatRepository(AppDbContext db)
//        {
//            _db = db;
//        }

//        public async Task<IEnumerable<Seat>> GetSeatsByEventAsync(int eventId)
//        {
//            return await _db.Seats
//                .Where(s => s.EventId == eventId)
//                .Include(s => s.Events)
//                .Include(s => s.Booking)
//                .ToListAsync();

//        }
//        public async Task<IEnumerable<Seat>> GetAllSeatsAsync()
//        {
//            return await _db.Seats
//                .Include(s => s.Events)
//                .Include(s => s.Booking)
//                .ToListAsync();

//        }
//        public async Task AddSeatsAsync(IEnumerable<Seat> seats)
//        {
//            await _db.Seats.AddRangeAsync(seats);
//            await _db.SaveChangesAsync();
//        }
//        public async Task UpdateSeatAsync(Seat seat)
//        {
//            _db.Seats.Update(seat);
//            await _db.SaveChangesAsync();
//        }

//        public async Task<(int total, int booked, int available)> GetSeatSummaryAsync(int eventId)
//        {
//            var total = await _db.Seats.CountAsync(s => s.EventId == eventId);
//            var booked = await _db.Seats.CountAsync(s => s.EventId == eventId && s.IsBooked);
//            var available = total - booked;
//            return (total, booked, available);
//        }


//    }
//}

