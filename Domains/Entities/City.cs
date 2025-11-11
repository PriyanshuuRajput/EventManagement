
namespace Domains.Entities
{
    public class City
    {
        public int Id { get; set; }
        public string CityName { get; set; } = null!;
        public Guid StateId { get; set; }

        public State State { get; set; } = null!;

        // ✅ Add this if venues belong to a city
        public ICollection<Venue> Venues { get; set; } = [];
    }
}