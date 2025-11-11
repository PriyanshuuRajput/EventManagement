using Domains.Entities;

namespace Applications.Interfaces.IRepository
{
    public interface ICountryRepository
    {
        // ✅ Get all countries
        Task<List<Country>> GetAllAsync();

        // ✅ Get country by Id
        Task<Country?> GetByIdAsync(Guid id);

        // ✅ Create a new country
        Task<Country> CreateAsync(Country country);

        // ✅ Update an existing country
        Task<Country?> UpdateAsync(Guid id, Country country);

        // ✅ Delete a country
        Task<bool> DeleteAsync(Guid id);

        // Optional: Check if country exists
        Task<bool> ExistsAsync(Guid id);

    }
}
