using Domains.Entities;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Applications.Dto
{
    public class EventDto 

    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Event name is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 50 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = " Please select a event type. " )]
        public int? EventCategoryId { get; set; }
        public string? CategorySlug { get; set; }
        public string? CategoryName { get; set; }
        public string? Description { get; set; } = "";
        public string? Language { get; set; } = "";


        [Required(ErrorMessage = "Event Duration is required. ")]
        public TimeSpan? Duration { get; set; }

        public string DurationInput
        {
            get => Duration == null ? "" : Duration.Value.ToString(@"hh\:mm");
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


        [Range(1, int.MaxValue, ErrorMessage = "Please select a venue.")]
        public int VenueId { get; set; }

        public string? VenueName { get; set; }

        public int? CityId { get; set; }

        public string? CityName { get; set; }

        public string Address { get; set; } = string.Empty;



        //[Required(ErrorMessage = "Ticket price is required.")]
        [Range(0, 10000, ErrorMessage = "Ticket price must be between 0 and 10,000.")]
        public decimal TicketPrice { get; set; }
        public string? ImageUrl { get; set; } = string.Empty;


        public IFormFile? ImageFile { get; set; }
        public int AvailableTickets { get; set; }

        public int TotalTickets { get; set; } = 0;
        public int SoldTickets { get; set; } = 0;

        public decimal ConvenienceFeePercent { get; set; } = 10;

        public decimal ConvenienceFeeAmount => Math.Round(TicketPrice * ConvenienceFeePercent / 100, 2);
        public decimal FinalPrice => TicketPrice + ConvenienceFeeAmount;
        public decimal GrossRevenue { get; set; }       
        public decimal CommissionAmount { get; set; }  
        public decimal ManagerPayout { get; set; }    

        public int? ManagerId { get; set; }
        public string? ManagerName { get; set; }

        public string? ManagerEmail { get; set; }

        public EventStatus Status { get; set; } = EventStatus.Pending;
        public string? AdminNote { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ApprovedAt { get; set; }
        public int Capacity { get; set; }


        //public decimal EventAmount { get; set; }

        //public decimal? OfferedEventAmount { get; set; }
        public bool IsAmountAccepted { get; set; }
        public bool PrizePaid { get; set; } = false;
        public DateTime? PrizePaidAt { get; set; }
        public bool IsPromoted { get; set; }

    }
}
