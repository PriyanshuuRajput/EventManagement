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
                .AsNoTracking()
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
    }
}
