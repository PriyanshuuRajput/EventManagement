
using Domains.Entities;

namespace Applications.Dto
{
    public class ManagerRevenueDto
    {
        public int EventId { get; set; }
        public string Title { get; set; } = string.Empty;

        public int TotalTickets { get; set; }
        public int TicketsSold { get; set; }
        public int TicketsRemaining { get; set; }

        public decimal Revenue { get; set; }
        public int BookingCount { get; set; }

        public EventStatus Status { get; set; }
    }
}
