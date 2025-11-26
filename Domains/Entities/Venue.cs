namespace Domains.Entities
{
    public class Venue
    {
        public int Id { get; set; }
        public string VenueName { get; set; } = null!;
        public string Address { get; set; } = null!;
        public int Capacity { get; set; }

        // 🔗 Foreign key to City
        public int CityId { get; set; }
        public City City { get; set; } = null!;

        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}