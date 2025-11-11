using Applications.Interfaces.IRepository;
using Domains.Entities;
using Infrastructures.DbContexts;
using Microsoft.EntityFrameworkCore;

public class CountryRepository : ICountryRepository
{
    private readonly AppDbContext _db;

    public CountryRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Country>> GetAllAsync()
    {
        return await _db.Countries.OrderBy(c => c.Name).ToListAsync();
    }

    public async Task<Country?> GetByIdAsync(Guid id)
    {
        return await _db.Countries.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Country> CreateAsync(Country country)
    {
        country.Id = Guid.NewGuid();
        await _db.Countries.AddAsync(country);
        await _db.SaveChangesAsync();
        return country;
    }

    public async Task<Country?> UpdateAsync(Guid id, Country country)
    {
        var existing = await _db.Countries.FindAsync(id);
        if (existing == null) return null;

        existing.Name = country.Name;
        existing.IsoCode = country.IsoCode;

        _db.Countries.Update(existing);
        await _db.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var country = await _db.Countries.FindAsync(id);
        if (country == null) return false;

        _db.Countries.Remove(country);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _db.Countries.AnyAsync(c => c.Id == id);
    }
}
