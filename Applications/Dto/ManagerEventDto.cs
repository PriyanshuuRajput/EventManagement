using Domains.Entities;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Applications.Dto
{
    public class ManagerEventDto
    {
        // Server will fill this on GET
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int Id { get; set; }

        // ====== SEND BY MANAGER ON CREATE ======
        [Required(ErrorMessage = "Event name is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 50 characters.")]
        public string Title { get; set; } = string.Empty;

        public string? EventType { get; set; } = string.Empty;

        public string? Description { get; set; } = "";

        [StringLength(50)]
        [Required(ErrorMessage = "Genre is required.")]

        public string? Genre { get; set; }

        public string? Language { get; set; } = "";

        //[Required(ErrorMessage = "Event Duration is required.")]
        public TimeSpan Duration { get; set; } = TimeSpan.Zero;


        [Required(ErrorMessage = "Event duration is required.")]
        public string DurationInput
        {
            get
            {
                if (Duration == TimeSpan.Zero)
                    return string.Empty;

                return Duration.ToString(@"hh\:mm");
            }
            set
            {
                // Try to convert 
                if (TimeSpan.TryParse(value, out var parsed))
                {
                    Duration = parsed;      // Save the converted value
                    DurationError = null;   // Clear the error
                }
                else
                {
                    // Save an error message (Blazor will show it)
                    DurationError = "Invalid duration format";
                }
            }
        }
        // This holds the error for Blazor to display
        public string? DurationError { get; set; }

        [Required(ErrorMessage = "Show date is required.")]
        [DataType(DataType.DateTime)]
        public DateTime StartDateOnly { get; set; }
        public DateTime EndDateOnly { get; set; }


        [Range(1, int.MaxValue, ErrorMessage = "Please select a venue")]
        public int VenueId { get; set; }

        public string? VenueName { get; set; }

        public int? CityId { get; set; }

        public string? CityName { get; set; }

        public string Address { get; set; } = string.Empty;

        [Range(0, 10000, ErrorMessage = "Ticket price must be between 0 and 10,000.")]
        public decimal TicketPrice { get; set; }

        // Manager uploads image
        public IFormFile? ImageFile { get; set; }

        // Server fills URL after upload
        public string? ImageUrl { get; set; }

        //// ======= SERVER/GET ONLY FIELDS =======
        //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        //public string? VenueName { get; set; }

        //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        //public int? CityId { get; set; }

        //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        //public string Address { get; set; } = string.Empty;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public EventStatus Status { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int TotalTickets { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int SoldTickets { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public decimal EventAmount { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public decimal? OfferedEventAmount { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? AdminNote { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]

        public DateTime CreatedAt { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public DateTime? ApprovedAt { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? ManagerName { get; set; }
    }
}
