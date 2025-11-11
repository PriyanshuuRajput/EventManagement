using System.ComponentModel.DataAnnotations;

namespace Applications.Dto
{
    public class CityDto
    {
        public int Id { get; set; }  // ✅ matches entity

        [Required]
        [StringLength(100)]
        public string CityName { get; set; } = string.Empty;

        [Required]
        public Guid StateId { get; set; }  // ✅ matches entity

        public string? StateName { get; set; }
        public Guid? CountryId { get; set; }
        public string? CountryName { get; set; }


        public int VenueCount { get; set; } = 0;
        public int EventCount { get; set; } = 0;
    }
}
