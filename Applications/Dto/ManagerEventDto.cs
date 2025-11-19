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
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string EventType { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [StringLength(50)]
        public string Genre { get; set; } = string.Empty;

        [StringLength(50)]
        public string Language { get; set; } = string.Empty;

        [Required]
        public TimeSpan Duration { get; set; } = TimeSpan.FromHours(1);

        [Required]
        public DateTime ShowDate { get; set; }

        [Required]
        public int VenueId { get; set; }

        [Required]
        [Range(0, 10000)]
        public decimal TicketPrice { get; set; }

        // Manager uploads image
        public IFormFile? ImageFile { get; set; }

        // Server fills URL after upload
        public string? ImageUrl { get; set; }

        // ======= SERVER/GET ONLY FIELDS =======
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? VenueName { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? CityId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Address { get; set; } = string.Empty;

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
