namespace Domains.Entities
{
    public class Country
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string IsoCode { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public ICollection<State> States { get; set; } = [];
    }
}
