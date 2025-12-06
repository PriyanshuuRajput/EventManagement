using System.ComponentModel.DataAnnotations;

namespace Applications.Dto
{
    public class VenueDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Venue name is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters.")]

        public string VenueName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Capacity is required.")]
        [Range(1, 100000, ErrorMessage = "Capacity must be greater than 0.")]
        public int Capacity { get; set; }

        [Required(ErrorMessage = "City is required.")]
        public int CityId { get; set; }

        public string CityName { get; set; } = string.Empty;

        public int EventCount { get; set; } = 0;

    }
}
