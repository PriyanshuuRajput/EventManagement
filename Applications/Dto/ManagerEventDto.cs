using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Applications.Dto
{
    public class ManagerEventDto
    {

        [Display(Name = "Event Name")]
        [Required(ErrorMessage = "Event title is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 50 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Event type is required.")]
        [StringLength(20, ErrorMessage = "Event type must be 20 characters.")]
        public string EventType { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Description cannot exceed 100 characters.")]
        public string Description { get; set; } = string.Empty;

        [StringLength(50)]
        public string Genre { get; set; } = string.Empty;

        [StringLength(50)]
        public string Language { get; set; } = string.Empty;

        [Required(ErrorMessage = "Event duration is required.")]
        public TimeSpan Duration { get; set; } = TimeSpan.FromHours(1);


        [Required(ErrorMessage = "Show date is required.")]
        [DataType(DataType.DateTime)]
        public DateTime ShowDate { get; set; }
        public int VenueId { get; set; }

        //[Required]
        //[Range(0, 10000)]
        //public decimal TicketPrice { get; set; }

        public IFormFile? ImageFile { get; set; }
        public string? ImageUrl { get; set; }


        public decimal? OfferedEventAmount { get; set; }
    }
}
