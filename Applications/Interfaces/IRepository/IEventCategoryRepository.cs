using Domains.Entities;

namespace Applications.Interfaces.IRepository
{
    public interface IEventCategoryRepository
    {
        Task<IEnumerable<EventCategory>> GetAllAsync();
        Task<EventCategory?> GetByIdAsync(int id);
        Task<EventCategory> AddAsync(EventCategory category);
        Task UpdateAsync(EventCategory category);
        Task DeleteAsync(int id);
    }
}
