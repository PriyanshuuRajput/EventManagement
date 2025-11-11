namespace Applications.Dto
{
    public class BookingRequest
    {
        public int EventId { get; set; }
        public List<int> SeatIds { get; set; } = new();
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
