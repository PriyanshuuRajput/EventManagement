using Applications.Dto;
using Applications.Interfaces.IRepository;
using Applications.Interfaces.IService;
using Domains.Entities;

namespace EventBooking_TicketManagement_API.Services
{
    public class SeatService : ISeatService
    {
        private readonly ISeatRepository _seatRepository;
        private readonly IEventRepository _eventRepository;

        public SeatService(ISeatRepository seatRepository, IEventRepository eventRepository)
        {
            _seatRepository = seatRepository;
            _eventRepository = eventRepository;
        }

        //  Get all seats (for Admin)
        public async Task<IEnumerable<SeatDto>> GetAllSeatsAsync()
        {
            var seats = await _seatRepository.GetAllSeatsAsync();

            return seats.Select(x => new SeatDto
            {
                Id = x.Id,
                SeatNumber = x.SeatNumber,
                Category = x.Category,
                Price = x.Price,
                IsBooked = x.IsBooked,
                EventId = x.EventId,
                BookingId = x.BookingId
            }).ToList();
        }

        //  Get all seats for an event (for User UI)
        public async Task<IEnumerable<SeatDto>> GetSeatsByEventAsync(int eventId)
        {
            var seats = await _seatRepository.GetSeatsByEventAsync(eventId);

            return seats.Select(s => new SeatDto
            {
                Id = s.Id,
                SeatNumber = s.SeatNumber,
                Category = s.Category,
                Price = s.Price,
                IsBooked = s.IsBooked,
                EventId = s.EventId,
                BookingId = s.BookingId
            }).ToList();
        }

        // Add seats for a specific event
        public async Task AddSeatsAsync(int eventId, int totalSeats, string category, decimal price)
        {

            var eventExists = await _eventRepository.EventExistsAsync(eventId);
            if (!eventExists)
                throw new Exception($"Event with Id {eventId} does not exist.");

            var seats = new List<Seat>();

            for (int i = 1; i <= totalSeats; i++)
            {
                seats.Add(new Seat
                {
                    SeatNumber = $"{category[0]}-{i:D2}", // e.g., G-01
                    Category = category.Trim(),
                    EventId = eventId,
                    IsBooked = false,
                    Price = price
                });
            }

            await _seatRepository.AddSeatsAsync(seats);
        }

        // ✅Update seat booking status
        public async Task UpdateSeatBookingAsync(int seatId, bool isBooked, int? bookingId = null)
        {
            var allSeats = await _seatRepository.GetAllSeatsAsync();
            var seat = allSeats.FirstOrDefault(s => s.Id == seatId);

            if (seat == null)
                throw new Exception($"Seat with ID {seatId} not found.");

            seat.IsBooked = isBooked;
            seat.BookingId = bookingId;

            await _seatRepository.UpdateSeatAsync(seat);
        }

        //  Get seat availability summary
        public async Task<(int total, int booked, int available)> GetSeatSummaryAsync(int eventId)
        {
            return await _seatRepository.GetSeatSummaryAsync(eventId);
        }
    }
}
