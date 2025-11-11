using Applications.Interfaces.IRepository;
using Domains.Entities;
using Infrastructures.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructures.Repository
{
    public class CityRepository : ICityRepository
    {
        private readonly AppDbContext _context;

        public CityRepository(AppDbContext context)
        {
            _context = context;
        }

        // ✅ Get all cities (including their State & Venues)
        public async Task<IEnumerable<City>> GetAllAsync()
        {
            return await _context.Cities
                .Include(c => c.State)           // Include related State
                .ThenInclude(s => s.Country)     // Include related Country (if you have navigation)
                .Include(c => c.Venues)          // Include related Venues
                .ToListAsync();
        }

        // ✅ Get a city by Id
        public async Task<City?> GetByIdAsync(int id)
        {
            return await _context.Cities
                .Include(c => c.State)
                .ThenInclude(s => s.Country)
                .Include(c => c.Venues)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        // ✅ Add a new city
        public async Task AddAsync(City city)
        {
            await _context.Cities.AddAsync(city);
            await _context.SaveChangesAsync();
        }

        // ✅ Update an existing city
        public async Task UpdateAsync(City city)
        {
            _context.Cities.Update(city);
            await _context.SaveChangesAsync();
        }

        // ✅ Delete a city by Id
        public async Task DeleteAsync(int id)
        {
            var city = await _context.Cities.FindAsync(id);
            if (city != null)
            {
                _context.Cities.Remove(city);
                await _context.SaveChangesAsync();
            }
        }
    }
}
