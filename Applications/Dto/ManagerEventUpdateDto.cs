using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

public class ManagerEventUpdateDto
{
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 50 characters.")]
    public string Title { get; set; } = string.Empty;
    public string? EventType { get; set; } = string.Empty;

    public string? Description { get; set; }

    [StringLength(50)]
    [Required(ErrorMessage = "Genre is required.")]

    public string? Genre { get; set; } = string.Empty;

    public string? Language { get; set; }
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
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Please select a venue")]
    public int VenueId { get; set; }
    public int CityId { get; set; }

    [Range(0, 10000, ErrorMessage = "Ticket price must be between 0 and 10,000.")]
    public decimal TicketPrice { get; set; }

    public IFormFile? ImageFile { get; set; }
    public string? ImageUrl { get; set; }
}
