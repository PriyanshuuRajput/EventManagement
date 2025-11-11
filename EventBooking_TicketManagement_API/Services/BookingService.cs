using Applications.Dto;
using Applications.Interfaces.IRepository;
using Applications.Interfaces.IService;
using Domains.Entities;

namespace EventBooking_TicketManagement_API.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ISeatRepository _seatRepository;
        private readonly IEventRepository _eventRepository;

        public BookingService(
            IBookingRepository bookingRepository,
            ISeatRepository seatRepository,
            IEventRepository eventRepository)
        {
            _bookingRepository = bookingRepository;
            _seatRepository = seatRepository;
            _eventRepository = eventRepository;
        }

        public async Task<BookingDto> CreateBookingAsync(BookingRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.SeatIds == null || !request.SeatIds.Any())
                throw new Exception("No seats selected for booking.");

            // Validate Event exists
            var eventExists = await _eventRepository.EventExistsAsync(request.EventId);
            if (!eventExists)
                throw new Exception($"Event with Id {request.EventId} does not exist.");

            //  Get all seats for the event
            var eventSeats = await _seatRepository.GetSeatsByEventAsync(request.EventId);
            var selectedSeats = eventSeats.Where(s => request.SeatIds.Contains(s.Id)).ToList();

            if (!selectedSeats.Any())
                throw new Exception("Selected seats are invalid or not part of this event.");

            //  Check for already booked seats
            var alreadyBooked = selectedSeats.Where(s => s.IsBooked).Select(s => s.SeatNumber).ToList();
            if (alreadyBooked.Any())
            {
                var list = string.Join(", ", alreadyBooked);
                throw new Exception($"The following seats are already booked: {list}");
            }

            //  Calculate total
            var totalAmount = selectedSeats.Sum(s => s.Price);

            //Create booking
            var booking = new Booking
            {
                EventId = request.EventId,
                UserName = request.UserName,
                UserEmail = request.UserEmail,
                BookingDate = DateTime.UtcNow,
                TicketNumber = Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                PaymentStatus = PaymentStatus.Pending,
                TotalAmount = totalAmount,
                Seats = selectedSeats
            };

            //  Mark seats as booked
            foreach (var seat in selectedSeats)
            {
                seat.IsBooked = true;
                seat.Booking = booking;
            }

            var savedBooking = await _bookingRepository.CreateBookingAsync(booking);

            // Map to DTO for response
            return new BookingDto
            {
                Id = savedBooking.Id,
                EventId = savedBooking.EventId,
                EventName = savedBooking.Event?.Title ?? "Unknown Event",
                SeatIds = selectedSeats.Select(s => s.Id).ToList(),
                Quantity = selectedSeats.Count,
                TotalAmount = totalAmount,
                BookingDate = savedBooking.BookingDate,
                UserName = savedBooking.UserName,
                UserEmail = savedBooking.UserEmail,
                TicketNumber = savedBooking.TicketNumber,
                PaymentStatus = savedBooking.PaymentStatus
            };
        }


        public async Task<IEnumerable<BookingDto>> GetAllBookingAsync()
        {
            var bookings = await _bookingRepository.GetAllBookingAsync();

            return bookings.Select(b => new BookingDto
            {
                Id = b.Id,
                EventId = b.EventId,
                EventName = b.Event?.Title ?? "Unknown Event",
                UserEmail = b.UserEmail,
                UserName = b.UserName,
                BookingDate = b.BookingDate,
                TicketNumber = b.TicketNumber,
                PaymentStatus = b.PaymentStatus,
                SeatIds = b.Seats?.Select(s => s.Id).ToList() ?? new List<int>(),
                Quantity = b.Seats?.Count ?? 0,
                TotalAmount = b.Seats?.Sum(s => s.Price) ?? 0
            });
        }

        public async Task<IEnumerable<BookingDto>> GetBookingByUserAsync(string userEmail)
        {
            var bookings = await _bookingRepository.GetBookingsByUserAsync(userEmail);

            return bookings.Select(b => new BookingDto
            {
                Id = b.Id,
                EventId = b.EventId,
                EventName = b.Event?.Title ?? "Unknown Event",
                UserEmail = b.UserEmail,
                UserName = b.UserName,
                BookingDate = b.BookingDate,
                TicketNumber = b.TicketNumber,
                PaymentStatus = b.PaymentStatus,
                SeatIds = b.Seats?.Select(s => s.Id).ToList() ?? new(),
                Quantity = b.Seats?.Count ?? 0,
                TotalAmount = b.Seats?.Sum(s => s.Price) ?? 0
            });
        }

        public async Task CancelBookingAsync(int bookingId)
        {
            var booking = await _bookingRepository.GetBookingByIdAsync(bookingId);

            if (booking == null)
                throw new Exception($"Booking not found");

            if (booking.PaymentStatus == PaymentStatus.Paid)
                throw new Exception("Cannot cancel a completed booking.");

            booking.PaymentStatus = PaymentStatus.Cancelled;

            await _bookingRepository.ReleaseSeatsAsync(bookingId);
        }
    }
}
