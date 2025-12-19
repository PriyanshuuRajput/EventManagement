using Applications.Dto;
using Applications.Interfaces.IRepository;
using Applications.Interfaces.IService;
using Domains.Entities;

namespace EventBooking_TicketManagement_API.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IEventRepository _eventRepository;

        public BookingService(
            IBookingRepository bookingRepository,
            IEventRepository eventRepository)
        {
            _bookingRepository = bookingRepository;
            _eventRepository = eventRepository;
        }

        public async Task<BookingDto> CreateBookingAsync(BookingRequest request , int userId)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.TicketCount <= 0)
                throw new Exception("Ticket count must be greater than zero.");

            // 1 Get Event
            var evnt = await _eventRepository.GetByIdAsync(request.EventId)
                       ?? throw new Exception($"Event with Id {request.EventId} does not exist.");

            // 2️ Check availability
            int availableTickets =
                evnt.TotalTickets - (evnt.SoldTickets + evnt.ReservedTickets);

            if (request.TicketCount > availableTickets)
                throw new Exception("Not enough tickets available.");

            // 3️ Reserve tickets
            evnt.ReservedTickets += request.TicketCount;
            await _eventRepository.UpdateAsync(evnt);

            // 4️ Create booking
            var booking = new Booking
            {
                EventId = request.EventId,
                UserId = userId, 
                TicketCount = request.TicketCount,
                BookingDate = DateTime.UtcNow,
                TicketNumber = Guid.NewGuid().ToString("N")[..8].ToUpper(),
                PaymentStatus = PaymentStatus.Pending
            };


            var savedBooking = await _bookingRepository.CreateBookingAsync(booking);

            // 5️ Return DTO
            return new BookingDto
            {
                Id = savedBooking.Id,
                EventId = savedBooking.EventId,
                EventName = evnt.Title,
                TicketCount = savedBooking.TicketCount,
                BookingDate = savedBooking.BookingDate,
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
                TicketCount = b.TicketCount,
                BookingDate = b.BookingDate,
                TicketNumber = b.TicketNumber,
                PaymentStatus = b.PaymentStatus
            });
        }

        public async Task<IEnumerable<BookingDto>> GetBookingByUserAsync(int userId)
        {
            var bookings = await _bookingRepository.GetBookingsByUserAsync(userId);

            return bookings.Select(b => new BookingDto
            {
                Id = b.Id,
                EventId = b.EventId,
                EventName = b.Event?.Title ?? "Unknown Event",
                TicketCount = b.TicketCount,
                BookingDate = b.BookingDate,
                TicketNumber = b.TicketNumber,
                PaymentStatus = b.PaymentStatus
            });
        }

        public async Task CancelBookingAsync(int bookingId)
        {
            var booking = await _bookingRepository.GetBookingByIdAsync(bookingId)
                          ?? throw new Exception("Booking not found.");

            if (booking.PaymentStatus == PaymentStatus.Paid)
                throw new Exception("Cannot cancel a paid booking.");

            var evnt = await _eventRepository.GetByIdAsync(booking.EventId);

            // Release reserved tickets safely
            evnt.ReservedTickets = Math.Max(0, evnt.ReservedTickets - booking.TicketCount);
            booking.PaymentStatus = PaymentStatus.Cancelled;

            await _eventRepository.UpdateAsync(evnt);
            await _bookingRepository.UpdateAsync(booking);
        }
    }
}
