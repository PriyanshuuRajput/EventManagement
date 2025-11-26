using Applications.Interfaces.IRepository;
using Domains.Entities;
using Infrastructures.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructures.Repository
{
    public class EventRepository : IEventRepository
    {
        private readonly AppDbContext _context;

        public EventRepository(AppDbContext context)
        {
            _context = context;
        }

        // ✅ Get all events with Venue, City, and Seats
        public async Task<IEnumerable<Event>> GetAllAsync()
        {
            return await _context.Events
                .Include(e => e.Venue)
                    .ThenInclude(v => v.City)
                .Include(e => e.Seats)
                .Include(e => e.Managers)
                    .ThenInclude(m => m.User)
                .AsNoTracking()
                .ToListAsync();
        }

        // ✅ Get a single event by Id (int)
        public async Task<Event?> GetByIdAsync(int id)
        {
            return await _context.Events
                .Include(e => e.Venue)
                    .ThenInclude(v => v.City)
                .Include(e => e.Seats)
                  .Include(e => e.Managers)
        .ThenInclude(m => m.User)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        // ✅ Get all events for a given venue
        public async Task<IEnumerable<Event>> GetEventsByVenueIdAsync(int venueId)
        {
            return await _context.Events
                .Where(e => e.VenueId == venueId)
                .Include(e => e.Venue)
                    .ThenInclude(v => v.City)
                .AsNoTracking()
                .ToListAsync();
        }

        // ✅ Add new event
        public async Task AddAsync(Event e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));

            await _context.Events.AddAsync(e);
            await _context.SaveChangesAsync();
        }

        // ✅ Update event
        public async Task UpdateAsync(Event e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));

            _context.Events.Update(e);
            _context.Entry(e).Property(x => x.CreatedAt).IsModified = false;
            await _context.SaveChangesAsync();
        }

        // ✅ Delete event by Id
        public async Task DeleteAsync(int id)
        {
            var existing = await _context.Events.FindAsync(id);
            if (existing != null)
            {
                _context.Events.Remove(existing);
                await _context.SaveChangesAsync();
            }
        }

        //public async Task<bool> EventExistAsync(int eventId)
        //{
        //    var exists = await _context.Events.AnyAsync(e => e.Id == eventId);
        //    Console.WriteLine($"Checking if Event {eventId} exists → {exists}");
        //    return exists;
        //}
        public async Task<bool> EventExistsAsync(int eventId)
        {
            return await _context.Events.AnyAsync(e => e.Id == eventId);
        }

        // Get events created by a specific Organizer
        public async Task<IEnumerable<Event>> GetEventsByManagerByIdAsync(int managerId)
        {
            return await _context.Events
                .Where(e => e.ManagerId == managerId)
                .Include(e => e.Venue)
                    .ThenInclude(v => v.City)
                      .Include(e => e.Managers)
        .ThenInclude(m => m.User)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
        }

        // Get events pending approval (for Admin)
        public async Task<IEnumerable<Event>> GetPendingEventsAsync()
        {
            return await _context.Events
                .Where(e => e.Status == EventStatus.Pending)
                .Include(e => e.Venue)
                    .ThenInclude(v => v.City)
                .OrderBy(e => e.CreatedAt)
                .ToListAsync();
        }

        // Get approved events (for Users)
        public async Task<IEnumerable<Event>> GetApprovedEventsAsync()
        {
            return await _context.Events
                .Where(e => e.Status == EventStatus.AdminApproved
                        || e.Status == EventStatus.PaymentConfirmed)
                .Include(e => e.Venue)
                    .ThenInclude(v => v.City)
                      .Include(e => e.Managers)
        .ThenInclude(m => m.User)
                .OrderByDescending(e => e.ShowDate)
                .ToListAsync();
        }

        // Get rejected events (for Admin/Organizer)
        public async Task<IEnumerable<Event>> GetRejectedEventsAsync()
        {
            return await _context.Events
                .Where(e => e.Status == EventStatus.Rejected)
                .Include(e => e.Venue)
                    .ThenInclude(v => v.City)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
        }


    }
}
