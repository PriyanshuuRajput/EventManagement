using Applications.Dto;
using Applications.Interfaces.IRepository;
using Applications.Interfaces.IService;
using Domains.Entities;
using EventBooking_TicketManagement_API.Helper;
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

            //// 2️ Check availability
            //int availableTickets =evnt.TotalTickets - (evnt.SoldTickets + evnt.ReservedTickets);

            //if (request.TicketCount > availableTickets)
            //    throw new Exception("Not enough tickets available.");

            // 🔒 USER LIMIT (MAX 10 PER EVENT)
            var userBookedTickets =
                await _bookingRepository.GetUserTicketCountByEventAsync(evnt.Id, userId);

            if (userBookedTickets + request.TicketCount > 10)
            {
                throw new Exception("You can book a maximum of 10 tickets for this event.");
            }

            var activeTickets = await _bookingRepository.GetActiveTicketCountByEventAsync(evnt.Id);

            var availableTickets = evnt.TotalTickets - activeTickets;

            if (request.TicketCount > availableTickets)
                throw new Exception("Not enough tickets available.");


            // 4️ Create booking
            var booking = new Booking
            {

                EventId = request.EventId,
                UserId = userId,
                TicketCount = request.TicketCount,
                CreatedAt = DateTime.UtcNow,
                TicketNumber = Guid.NewGuid().ToString("N")[..8].ToUpper(),
                PaymentStatus = PaymentStatus.Pending,
                QrCode = Guid.NewGuid().ToString(),
                UsedEntries = 0
            };

            var savedBooking = await _bookingRepository.CreateBookingAsync(booking);

            _ = Task.Run(async () =>
            {
                try
                {
                    var user = await _bookingRepository.GetUserByIdAsync(userId);

                    if (user != null && !string.IsNullOrWhiteSpace(user.Email))
                    {
                        var qrBytes = _qrCodeService.GenerateQr(savedBooking.QrCode);

                        var emailHtml = TicketTemplate.TicketHtml(
                            evnt.Title,
                            evnt.Venue?.VenueName ?? "Venue",
                            evnt.StartDate,
                            savedBooking.TicketCount,
                            savedBooking.TicketNumber,
                            evnt.TicketPrice * savedBooking.TicketCount,
                            "cid:ticketQr"
                        );

                        await _emailService.SendEmailWithQrAsync(
                            user.Email,
                            "🎟 Your Event Ticket – EventiGO",
                            emailHtml,
                            qrBytes,
                            "ticketQr"
                        );
                    }
                }
                catch (Exception ex)
                {
                    // LOG ONLY
                    Console.WriteLine($"Email failed: {ex.Message}");
                }
            });
            // 6️⃣ Return DTO
            return new BookingDto
            {
                Id = savedBooking.Id,
                EventId = savedBooking.EventId,
                EventName = evnt.Title,
                TicketCount = savedBooking.TicketCount,
                CreatedAt = savedBooking.CreatedAt,
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
                CreatedAt = b.CreatedAt,
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
                else if (b.Event != null && b.Event.EndDate < now)
                {
                    status = BookingStatus.Completed;
                }
                else if (b.Event == null)
                {
                    status = BookingStatus.Cancelled;
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
                    CreatedAt = b.CreatedAt,
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

            if (booking.PaymentStatus == PaymentStatus.Cancelled)
                return;

            booking.PaymentStatus = PaymentStatus.Cancelled;
            booking.CancelledAt = DateTime.UtcNow;

            await _bookingRepository.UpdateAsync(booking);

            _ = Task.Run(async () =>
            {
                try
                {
                    var user = await _bookingRepository.GetUserByIdAsync(booking.UserId);

                    if (user != null && !string.IsNullOrWhiteSpace(user.Email))
                    {
                        var emailBody = CancelTicketEmailTemplate.CancelHtml(
                            booking.Event!.Title,
                            booking.Event.Venue?.VenueName ?? "Venue",
                            booking.Event.StartDate,
                            booking.TicketNumber,
                            booking.TicketCount
                        );

                        await _emailService.SendEmailAsync(
                            user.Email,
                            "Booking Cancelled – EventiGO",
                            emailBody
                        );
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Cancel email failed: {ex.Message}");
                }
            });
        }

    }
}