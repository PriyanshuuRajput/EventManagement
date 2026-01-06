using Applications.Dto;
using Applications.Dto.Pagination;
using Applications.Interfaces.IRepository;
using Applications.Interfaces.IService;
using Domains.Entities;
using EventBooking_TicketManagement_API.Helper;
using EventBooking_TicketManagement_API.Helpers;
using Infrastructures.DbContexts;
using Microsoft.EntityFrameworkCore;


namespace EventBooking_TicketManagement_API.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IEmailService _emailService;
        private readonly IQrCodeService _qrCodeService;

        private readonly AppDbContext _db;
        public BookingService(
            IBookingRepository bookingRepository,
            IEventRepository eventRepository,
            IEmailService emailService,
            IQrCodeService qrCodeService,
            AppDbContext db)
        {
            _bookingRepository = bookingRepository;
            _eventRepository = eventRepository;
            _emailService = emailService;
            _qrCodeService = qrCodeService;
            _db = db;
        }


        public async Task<BookingDto> CreateBookingAsync(BookingRequest request, int userId)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.TicketCount <= 0)
                throw new Exception("Ticket count must be greater than zero.");

            var evnt = await _eventRepository.GetByIdAsync(request.EventId)
                       ?? throw new Exception("Event does not exist.");

            // USER LIMIT
            var userBookedTickets =
                await _bookingRepository.GetUserTicketCountByEventAsync(evnt.Id, userId);

            if (userBookedTickets + request.TicketCount > 10)
                throw new Exception("You can book a maximum of 10 tickets for this event.");

            var activeTickets =
                await _bookingRepository.GetActiveTicketCountByEventAsync(evnt.Id);

            if (request.TicketCount > evnt.TotalTickets - activeTickets)
                throw new Exception("Not enough tickets available.");

            using var tx = await _db.Database.BeginTransactionAsync();

            try
            {
                var booking = new Booking
                {
                    EventId = request.EventId,
                    UserId = userId,
                    TicketCount = request.TicketCount,
                    CreatedAt = DateTime.UtcNow,
                    TicketNumber = Guid.NewGuid().ToString("N")[..8].ToUpper(),
                    PaymentStatus = PaymentStatus.Paid,
                    QrCode = Guid.NewGuid().ToString(),
                    UsedEntries = 0
                };

                var savedBooking = await _bookingRepository.CreateBookingAsync(booking);

                evnt.SoldTickets += request.TicketCount;
                await _eventRepository.UpdateAsync(evnt);

                await tx.CommitAsync();

                SendTicketEmailAsync(savedBooking, evnt, userId);

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
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        private void SendTicketEmailAsync(Booking booking, Event evnt, int userId)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    var user = await _bookingRepository.GetUserByIdAsync(userId);
                    if (user == null || string.IsNullOrWhiteSpace(user.Email))
                        return;

                    var qrBytes = _qrCodeService.GenerateQr(booking.QrCode);

                    var emailHtml = TicketTemplate.TicketHtml(
                        evnt.Title,
                        evnt.Venue?.VenueName ?? "Venue",
                        evnt.StartDate,
                        booking.TicketCount,
                        booking.TicketNumber,
                        evnt.TicketPrice * booking.TicketCount,
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
                catch (Exception ex)
                {
                    Console.WriteLine($"Email failed: {ex.Message}");
                }
            });
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
                    ManagerId = b.Event?.ManagerId ,
                    ManagerName = b.Event?.Managers != null? 
                                    b.Event.Managers.ManagerName
                                    : "Admin",
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

            var evnt = await _eventRepository.GetByIdAsync(booking.EventId);
            if (evnt != null)
            {
                evnt.SoldTickets -= booking.TicketCount;
                if (evnt.SoldTickets < 0)
                    evnt.SoldTickets = 0;

                await _eventRepository.UpdateAsync(evnt);
            }


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

        public async Task<PagedResult<BookingDto>> GetBookingsByManagerAsync(int managerId,PagedRequest request)
        {
            return await _bookingRepository
                .GetBookingsByManagerIdAsync(managerId, request);
        }
        public async Task<PagedResult<BookingDto>> GetAllBookingsForAdminAsync(PagedRequest request)
        {
            var query = _db.Bookings
    .Include(b => b.Event)
        .ThenInclude(e => e.Managers)
    .Include(b => b.Event)
        .ThenInclude(e => e.Venue)
    .Include(b => b.Event)
        .ThenInclude(e => e.EventCategory)
    .AsQueryable();
            //  Search
            if (!string.IsNullOrWhiteSpace(request.Search))
                query = query.Where(b => b.Event.Title.Contains(request.Search));

            //  Date filter
            if (request.DateFilter.HasValue)
                query = query.Where(b => b.CreatedAt.Date == request.DateFilter.Value.Date);

            //  Total count 
            var totalCount = await query.CountAsync();

            //  Page data
            var items = await query
                .OrderByDescending(b => b.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(b => new BookingDto
                {
                    Id = b.Id,
                    EventId = b.EventId,
                    EventName = b.Event.Title,
                    TicketCount = b.TicketCount,
                    TicketPrice = b.Event.TicketPrice,
                    CreatedAt = b.CreatedAt,
                    Status = b.PaymentStatus == PaymentStatus.Cancelled
                        ? BookingStatus.Cancelled
                        : DateTime.UtcNow > b.Event.EndDate
                            ? BookingStatus.Completed
                            : BookingStatus.Upcoming
                })
                .ToListAsync();

            //  Return paged result
            return new PagedResult<BookingDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }
        public async Task<PagedResult<BookingDto>> GetAdminBookingsAsync(PagedRequest request)
        {
            return await _bookingRepository.GetBookingsForAdminAsync(request);
        }

    }
}
