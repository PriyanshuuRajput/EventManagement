using Domains.Entities;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Applications.Dto
{
    public class EventDto : IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Event name is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 50 characters.")]
        public string Title { get; set; } = string.Empty;
        public string? CategorySlug { get; set; }
        public EventCategory? EventCategory { get; set; }

        public string? EventType { get; set; } = string.Empty;

        public string? Description { get; set; } = "";


        [StringLength(50)]
        [Required(ErrorMessage = "Genre is required.")]
        public string? Genre { get; set; } = string.Empty;
        public string? Language { get; set; } = "";

        public TimeSpan? Duration { get; set; }


        [Required(ErrorMessage = "Event duration is required.")]
        public string DurationInput
        {
            get => Duration== null?"":Duration.Value.ToString(@"hh\:mm");
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    Duration = null;
                    return;
                }

                if (TimeSpan.TryParse(value, out var parsed))
                {
                    Duration = parsed;
                    DurationError = null;
                }
                else
                {
                    DurationError = "Invalid duration format (use HH:mm)";
                }
            }
        }
        public string? DurationError { get; set; }

        public DateTime StartDateOnly { get; set; } = DateTime.Today;
        public TimeSpan StartTime { get; set; } = new TimeSpan(18, 0, 0);

        public DateTime? EndDateOnly { get; set; }
        public TimeSpan EndTime { get; set; } = new TimeSpan(18, 0, 0);

        // Computed final values
        public DateTime StartDate => StartDateOnly.Date + StartTime;

        public DateTime EndDate =>
            (EndDateOnly ?? StartDateOnly).Date + EndTime;


        [Range(1, int.MaxValue, ErrorMessage = "Please select a venue")]
        public int VenueId { get; set; }

        public string? VenueName { get; set; }

        public int? CityId { get; set; }

        public string? CityName { get; set; }

        public string Address { get; set; } = string.Empty;



        //[Required(ErrorMessage = "Ticket price is required.")]
        [Range(0, 10000, ErrorMessage = "Ticket price must be between 0 and 10,000.")]
        public decimal TicketPrice { get; set; }

        //[StringLength(500, ErrorMessage = "Image URL cannot exceed 500 characters.")]
        //[Required(ErrorMessage = "Event image is required.")]
        public string ImageUrl { get; set; } = string.Empty;


        public IFormFile? ImageFile { get; set; }

        public int TotalTickets { get; set; } = 0;
        public int SoldTickets { get; set; } = 0;

        public int? ManagerId { get; set; }
        public string ManagerName { get; set; } = string.Empty;

        public string? ManagerEmail { get; set; }

        public EventStatus Status { get; set; } = EventStatus.Pending;
        public string? AdminNote { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ApprovedAt { get; set; }
        public int Capacity { get; set; }


        public decimal EventAmount { get; set; }

        public decimal? OfferedEventAmount { get; set; }
        public bool IsAmountAccepted { get; set; }
        public bool PrizePaid { get; set; } = false;
        public DateTime? PrizePaidAt { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            string eventType = EventType?.Trim() ?? "";
            string genre = Genre?.Trim() ?? "";

            if (eventType == "" && genre == "")
            {
                yield return new ValidationResult(
                    "Please provide at least Event Type or Genre.",
                    new[] { nameof(EventType), nameof(Genre) });

            }
            if (eventType == "" && genre != "")
            {
                EventType = genre;
            }
            if (genre == "" && eventType != "")
            {
                Genre = eventType;
            }
        }
    }
}
