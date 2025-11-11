using Domains.Entities;

namespace Applications.Interfaces.IRepository
{
    public interface IEventRepository
    {
        Task<IEnumerable<Event>> GetAllAsync();
        Task<Event?> GetByIdAsync(int id);
        Task<IEnumerable<Event>> GetEventsByVenueIdAsync(int venueId);
        Task AddAsync(Event ev);
        Task UpdateAsync(Event ev);
        Task DeleteAsync(int id);

        Task<bool> EventExistsAsync(int eventId);
    }
}
