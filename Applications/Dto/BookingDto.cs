using Domains.Entities;

namespace Applications.Dto
{
    public class BookingDto
    {
        public int Id { get; set; }

        public int EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public DateTime EventStartDate { get; set; }
        public DateTime EventEndDate { get; set; }
        public decimal TicketPrice { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string VenueName { get; set; } = string.Empty;

        // UI helpers
        public decimal TotalAmount => TicketPrice * TicketCount;
        // Ticket info
        public int TicketCount { get; set; }

        // Booking info
        public DateTime BookingDate { get; set; }
        public string TicketNumber { get; set; } = string.Empty;

        // Payment
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

        public string QrCode { get;set; } = string.Empty;
        public BookingStatus Status { get; set; }

    }


    public enum BookingStatus
    {
        Upcoming,
        Completed,
        Cancelled
    }


}
