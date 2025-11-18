using Microsoft.AspNetCore.Http;

public class ManagerEventUpdateDto
{
    public string Title { get; set; }
    public string EventType { get; set; }
    public string Description { get; set; }
    public string Genre { get; set; }
    public string Language { get; set; }
    public TimeSpan Duration { get; set; }
    public DateTime ShowDate { get; set; }

    public int VenueId { get; set; }
    public int CityId { get; set; }

    public decimal TicketPrice { get; set; }

    public IFormFile? ImageFile { get; set; }
    public string? ImageUrl { get; set; }
}
