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
    public TimeSpan? Duration { get; set; }

    [Required(ErrorMessage = "Event duration is required.")]
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

    [Range(1, int.MaxValue, ErrorMessage = "Please select a venue")]
    public int VenueId { get; set; }
    public int CityId { get; set; }

    [Range(0, 10000, ErrorMessage = "Ticket price must be between 0 and 10,000.")]
    public decimal TicketPrice { get; set; }

    public IFormFile? ImageFile { get; set; }
    public string? ImageUrl { get; set; }
}
