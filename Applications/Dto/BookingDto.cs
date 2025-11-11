using Domains.Entities;

namespace Applications.Dto
{


    public class BookingDto
    {
        public int Id { get; set; }

        public int EventId { get; set; }
        public string EventName { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public List<int> SeatIds { get; set; } = new();

        public DateTime BookingDate { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string UserEmail { get; set; } = string.Empty;

        public string TicketNumber { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }

        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    }
}
