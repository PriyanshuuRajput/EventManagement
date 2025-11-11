namespace Domains.Entities
{
    public class State
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public Guid CountryId { get; set; }

        public Country Country { get; set; } = null!;
        public ICollection<City> Cities { get; set; } = [];
    }
}