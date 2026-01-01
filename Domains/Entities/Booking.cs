
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
    public enum PaymentMode
    {
        None,
        UPI,
        Card,
        NetBanking,
        Wallet
    }
    public class Booking
    {
        public int Id { get; set; }

        // Relationship
        public int EventId { get; set; }
        public Event Event { get; set; }

        //Relationship
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public AdminUser AdminUser { get; set; }
        public int TicketCount { get; set; }
        public string TicketNumber { get; set; } = string.Empty;
        public string QrCode { get; set; } = string.Empty;

        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
        public PaymentMode? PaymentMode { get; set; } 

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CancelledAt { get; set; }
        public int UsedEntries { get; set; } 

    }

}
