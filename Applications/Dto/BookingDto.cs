using Domains.Entities;

namespace Applications.Dto
{
    public class BookingDto
    {
        public int Id { get; set; }

        public int EventId { get; set; }
        public string EventName { get; set; } = string.Empty;

        // Ticket info
        public int TicketCount { get; set; }

        // Booking info
        public DateTime BookingDate { get; set; }
        public string TicketNumber { get; set; } = string.Empty;

        // Payment
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    }
}
