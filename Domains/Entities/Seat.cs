using System.ComponentModel.DataAnnotations.Schema;

namespace Domains.Entities
{
    public class Seat
    {
        public int Id { get; set; }
        public string SeatNumber { get; set; } = string.Empty;
        public string Category { get; set; } = "Regular";

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }


        public bool IsBooked { get; set; }

        public int EventId { get; set; }
        public Event? Events { get; set; }

        // Booking relationship (nullable)
        public int? BookingId { get; set; }
        public Booking? Booking { get; set; }

    }
}
