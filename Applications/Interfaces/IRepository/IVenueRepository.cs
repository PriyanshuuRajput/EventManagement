using Domains.Entities;

namespace Applications.Interfaces.IRepository
{
    public interface IVenueRepository
    {

        Task<IEnumerable<Venue>> GetAllAsync();
        Task<Venue?> GetByIdAsync(int id);
        Task AddAsync(Venue v);
        Task UpdateAsync(Venue v);
        Task DeleteAsync(int id);
    }
}
