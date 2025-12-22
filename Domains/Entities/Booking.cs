
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

        // Booking details
        public DateTime BookingDate { get; set; } = DateTime.UtcNow;
        public string TicketNumber { get; set; } = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

        public PaymentMode? PaymentMode { get; set; } 
        public  string QrCode {  get; set; } = string.Empty;
        public int UsedEntries { get; set; } 

    }

}
