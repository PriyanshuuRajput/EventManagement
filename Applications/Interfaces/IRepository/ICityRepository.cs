using Domains.Entities;

namespace Applications.Interfaces.IRepository
{
    public interface ICityRepository
    {
        Task<IEnumerable<City>> GetAllAsync();
        Task<City?> GetByIdAsync(int id);   // ✅ Keep int
        Task AddAsync(City city);
        Task UpdateAsync(City city);
        Task DeleteAsync(int id);           // ✅ Keep int
    }
}
