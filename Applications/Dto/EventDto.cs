using Domains.Entities;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Applications.Dto
{
    public class EventDto
    {
        public int Id { get; set; }

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

        public string? VenueName { get; set; }

        public int? CityId { get; set; }

        public string? CityName { get; set; }

        public string Address { get; set; } = string.Empty;



        [Required(ErrorMessage = "Ticket price is required.")]
        [Range(0, 10000, ErrorMessage = "Ticket price must be between 0 and 10,000.")]
        public decimal TicketPrice { get; set; }

        //[StringLength(500, ErrorMessage = "Image URL cannot exceed 500 characters.")]
        [DataType(DataType.ImageUrl)]
        public string ImageUrl { get; set; } = string.Empty;

        public IFormFile? ImageFile { get; set; }

        public int TotalTickets { get; set; } = 0;
        public int SoldTickets { get; set; } = 0;

        public int ManagerId { get; set; }
        public string ManagerName { get; set; } = string.Empty;

        public EventStatus Status { get; set; } = EventStatus.Pending;
        public string? AdminNote { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ApprovedAt { get; set; }

        public decimal EventAmount { get; set; }

        public decimal? OfferedEventAmount { get; set; }
        public bool IsAmountAccepted { get; set; }
    }
}
