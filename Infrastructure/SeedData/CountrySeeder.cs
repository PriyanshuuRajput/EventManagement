using Domains.Entities;
using Infrastructures.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;

public static class CountrySeeder
{
    private record CountrySeed(
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("iso2")] string? Iso2
    );

    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Countries.AnyAsync()) return;

        var filePath = Path.Combine(AppContext.BaseDirectory, "SeedData", "countries.json");
        if (!File.Exists(filePath))
            throw new FileNotFoundException("countries.json not found: " + filePath);

        var json = await File.ReadAllTextAsync(filePath);
        var list = JsonSerializer.Deserialize<List<CountrySeed>>(json);

        if (list == null || list.Count == 0) return;

        var countries = list
            .Where(x => !string.IsNullOrWhiteSpace(x.Name))
            .Select(x => new Country
            {
                Id = Guid.NewGuid(),
                Name = x.Name.Trim(),
                IsoCode = (x.Iso2 ?? string.Empty).Trim()
            })
            .GroupBy(c => c.Name) // prevent duplicates
            .Select(g => g.First())
            .OrderBy(c => c.Name)
            .ToList();

        db.Countries.AddRange(countries);
        await db.SaveChangesAsync();
    }
}
