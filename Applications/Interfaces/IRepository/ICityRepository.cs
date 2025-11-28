using Domains.Entities;

namespace Applications.Interfaces.IRepository
{
    public interface ICityRepository
    {
        Task<IEnumerable<City>> GetAllAsync();
        Task<City?> GetByIdAsync(int id);
        Task AddAsync(City city);
        Task<City?> GetCityByVenueIdAsync(int venueId);
        Task UpdateAsync(City city);
        Task DeleteAsync(int id);
    }
}
