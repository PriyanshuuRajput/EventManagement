using System.ComponentModel.DataAnnotations;

namespace Applications.Dto
{
    public class StateDto
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public Guid CountryId { get; set; }

        public string? CountryName { get; set; }
    }
}
