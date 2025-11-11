
using System.ComponentModel.DataAnnotations.Schema;

namespace Domains.Entities
{
    public enum PaymentStatus
    {
        Pending,
        Paid,
        Failed,
        Cancelled
    }
    public class Booking
    {
        public int Id { get; set; }

        // Relationship
        public int EventId { get; set; }
        public Event? Event { get; set; }

        // Relationship with seats
        public List<Seat> Seats { get; set; } = new();

        // User info
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;

        // Booking details
        public DateTime BookingDate { get; set; } = DateTime.UtcNow;
        public string TicketNumber { get; set; } = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }
        //public string PaymentStatus { get; set; } = "Pending"; // Pending / Paid / Cancelled

        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

    }

}
