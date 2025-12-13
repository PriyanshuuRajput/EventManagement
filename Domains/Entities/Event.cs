using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domains.Entities
{
    public enum EventStatus
    {
        Pending = 0,
        AdminApproved = 1,
        //PaymentConfirmed = 2,
        Rejected = 2
    }
    public class Event
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        //public string EventType { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        //public string Genre { get; set; } = string.Empty;

        public string Language { get; set; } = string.Empty;

        public TimeSpan? Duration { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TicketPrice { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

     
        [ForeignKey(nameof(Venue))]
        public int VenueId { get; set; }

        public Venue? Venue { get; set; }

        [ForeignKey(nameof(EventCategory))]
        public int? EventCategoryId { get; set; }
        public EventCategory? EventCategory { get; set; }

        // 🔗 Optional: Seats or bookings related to this event
        public ICollection<Seat>? Seats { get; set; }

        [ForeignKey(nameof(Managers))]
        public int? ManagerId { get; set; }

        public Manager? Managers { get; set; }

        public EventStatus Status { get; set; } = EventStatus.Pending;

        public string? AdminNote { get; set; }


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ApprovedAt { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal EventAmount { get; set; } = 0m;

        [Column(TypeName = "decimal(18,2)")]
        public decimal? OfferedEventAmount { get; set; }

        public bool IsAmountAccepted { get; set; } = false;

        public bool IsPrizePaid { get; set; } = false;

        public DateTime? PrizePaidAt { get; set; }
        //public string ManagerName { get; set; }
        public int TotalTickets { get; set; }
        public int SoldTickets { get; set; }
    }
}
