using Applications.Dto;
using Applications.Interfaces.IRepository;
using Applications.Interfaces.IService;
using Domains.Entities;
using EventBooking_TicketManagement_API.Helpers;


namespace EventBooking_TicketManagement_API.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IEmailService _emailService;
        private readonly IQrCodeService _qrCodeService;
        


        public BookingService(
     IBookingRepository bookingRepository,
     IEventRepository eventRepository,
     IEmailService emailService,
     IQrCodeService qrCodeService
            )
        {
            _bookingRepository = bookingRepository;
            _eventRepository = eventRepository;
            _emailService = emailService;
            _qrCodeService = qrCodeService;

        }

        public async Task<BookingDto> CreateBookingAsync(BookingRequest request, int userId)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.TicketCount <= 0)
                throw new Exception("Ticket count must be greater than zero.");

            // 1️ Get Event
            var evnt = await _eventRepository.GetByIdAsync(request.EventId)
                       ?? throw new Exception("Event does not exist.");

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
                PaymentStatus = PaymentStatus.Pending,
                QrCode = Guid.NewGuid().ToString(),
                UsedEntries = 0
            };

            var savedBooking = await _bookingRepository.CreateBookingAsync(booking);

            // 5️ EMAIL 
            try
            {
                var user = await _bookingRepository.GetUserByIdAsync(userId);

                if (user != null && !string.IsNullOrWhiteSpace(user.Email))
                {
                    var scanUrl =
                        $"https://yourdomain.com/api/booking/scan?token={savedBooking.QrCode}";

                    var qrImage = _qrCodeService.GenerateQr(scanUrl);

                    var emailBody = EmailTemplates.BookingConfirmation(
                        evnt.Title,
                        savedBooking.TicketCount,
                        savedBooking.TicketNumber
                    );

                    await _emailService.SendEmailWithQrAsync(
                        user.Email,
                        "🎟 Booking Confirmed – EventiGO",
                        emailBody,
                        qrImage
                    );
                }
            }
            catch (Exception ex)
            {
                // LOG ONLY — booking should not fail
                Console.WriteLine($"Email failed: {ex.Message}");
            }

            // 6️⃣ Return DTO
            return new BookingDto
            {
                Id = savedBooking.Id,
                EventId = savedBooking.EventId,
                EventName = evnt.Title,
                TicketCount = savedBooking.TicketCount,
                BookingDate = savedBooking.BookingDate,
                TicketNumber = savedBooking.TicketNumber,
                PaymentStatus = savedBooking.PaymentStatus,
                QrCode = savedBooking.QrCode
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

            var now = DateTime.UtcNow;

            return bookings.Select(b =>
            {
                BookingStatus status;

                if (b.PaymentStatus == PaymentStatus.Cancelled)
                {
                    status = BookingStatus.Cancelled;
                }
                else if (b.Event.EndDate < now)
                {
                    status = BookingStatus.Completed;
                }
                else
                {
                    status = BookingStatus.Upcoming;
                }
                return new BookingDto
                {
                    Id = b.Id,
                    EventId = b.EventId,
                    EventName = b.Event?.Title ?? "Unknown Event",
                    EventStartDate = b.Event?.StartDate ?? DateTime.MinValue,
                    EventEndDate = b.Event?.EndDate ?? DateTime.MinValue,
                    TicketPrice = b.Event?.TicketPrice ?? 0,
                    ImageUrl = b.Event?.ImageUrl ?? string.Empty,
                    VenueName = b.Event?.Venue?.VenueName ?? "Venue not available",
                    TicketCount = b.TicketCount,
                    BookingDate = b.BookingDate,
                    TicketNumber = b.TicketNumber,
                    PaymentStatus = b.PaymentStatus,
                    QrCode = b.QrCode,
                    Status = status,
                };
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
