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

        public decimal TotalAmount => TicketPrice * TicketCount;
        // Ticket info
        public int TicketCount { get; set; }

        // Booking info
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CancelledAt { get; set; }
        public string TicketNumber { get; set; } = string.Empty;

        // Payment
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

        public string QrCode { get;set; } = string.Empty;
        public BookingStatus Status { get; set; }

        public decimal ConvenienceFeePercent { get; set; } = 10;

        public decimal ConvenienceFeePerTicket =>
            Math.Round(TicketPrice * ConvenienceFeePercent / 100, 2);

        public decimal PlatformFee =>
            ConvenienceFeePerTicket * TicketCount;

        public decimal ManagerEarning =>
            TicketPrice * TicketCount;

       public decimal GrossAmount => TicketPrice * TicketCount;

    }

    public enum BookingStatus
    {
        Upcoming,
        Completed,
        Cancelled
    }

}
