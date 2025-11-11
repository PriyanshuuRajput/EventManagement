using Domains.Entities;
using Infrastructures.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;

public static class StateSeeder
{
    private record StateSeed(
        [property: JsonPropertyName("name")] string CountryName,
        [property: JsonPropertyName("states")] List<string> States
    );

    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.States.AnyAsync())
        {
            Console.WriteLine("States table already has data. Skipping seeding.");
            return;
        }

        var filePath = Path.Combine(AppContext.BaseDirectory, "SeedData", "states.json");
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"states.json not found at: {filePath}");
            return;
        }

        var json = await File.ReadAllTextAsync(filePath);
        var stateSeeds = JsonSerializer.Deserialize<List<StateSeed>>(json);

        if (stateSeeds == null || stateSeeds.Count == 0)
        {
            Console.WriteLine("No states found in states.json. Nothing to seed.");
            return;
        }

        var countries = await db.Countries.ToListAsync();
        if (!countries.Any())
        {
            Console.WriteLine("No countries found in the database. Seed countries first.");
            return;
        }

        var statesToAdd = new List<State>();

        foreach (var seed in stateSeeds)
        {
            var country = countries.FirstOrDefault(c =>
                c.Name.Equals(seed.CountryName, StringComparison.OrdinalIgnoreCase));

            if (country == null)
            {
                Console.WriteLine($"Skipping country '{seed.CountryName}': not found in database.");
                continue;
            }

            foreach (var stateName in seed.States)
            {
                if (string.IsNullOrWhiteSpace(stateName)) continue;

                statesToAdd.Add(new State
                {
                    Id = Guid.NewGuid(),
                    Name = stateName.Trim(),
                    CountryId = country.Id
                });
            }
        }

        statesToAdd = statesToAdd
            .GroupBy(s => new { s.Name, s.CountryId })
            .Select(g => g.First())
            .OrderBy(s => s.Name)
            .ToList();

        if (!statesToAdd.Any())
        {
            Console.WriteLine("No valid states to insert after filtering duplicates.");
            return;
        }

        db.States.AddRange(statesToAdd);
        await db.SaveChangesAsync();

        Console.WriteLine($"Seeded {statesToAdd.Count} states successfully.");
    }
}
