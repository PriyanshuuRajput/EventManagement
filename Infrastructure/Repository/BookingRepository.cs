using Applications.Dto;
using Applications.Dto.Pagination;
using Applications.Interfaces.IRepository;
using Domains.Entities;
using Infrastructures.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class BookingRepository : IBookingRepository
    {
        private readonly AppDbContext _db;

        public BookingRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Booking> CreateBookingAsync(Booking booking)
        {
            _db.Bookings.Add(booking);
            await _db.SaveChangesAsync();
            return booking;
        }

        public async Task<IEnumerable<Booking>> GetAllBookingAsync()
        {
            return await _db.Bookings
                .Include(b => b.Event)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<Booking?> GetBookingByIdAsync(int id)
        {
            return await _db.Bookings
                .Include(b => b.Event)
                    .ThenInclude(e => e.Venue)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<Booking>> GetBookingsByUserAsync(int userId)
        {
            return await _db.Bookings
                .Include(b => b.Event)
                    .ThenInclude(e => e.Venue)
                .Where(b => b.UserId == userId && b.Event != null)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }
        public async Task<int> GetActiveTicketCountByEventAsync(int eventId)
        {
            return await _db.Bookings
                .Where(b =>
                    b.EventId == eventId &&
                    b.PaymentStatus != PaymentStatus.Cancelled)
                .SumAsync(b => b.TicketCount);
        }

        public async Task<int> GetUserTicketCountByEventAsync(int eventId, int userId)
        {
            return await _db.Bookings
                .Where(b =>
                    b.EventId == eventId &&
                    b.UserId == userId &&
                    b.PaymentStatus != PaymentStatus.Cancelled)
                .SumAsync(b => b.TicketCount);
        }

        public async Task UpdateAsync(Booking booking)
        {
            _db.Bookings.Update(booking);
            await _db.SaveChangesAsync();
        }

        public async Task<AdminUser?> GetUserByIdAsync(int userId)
        {
            return await _db.Users
                .FirstOrDefaultAsync(b => b.Id == userId);
        }
        public async Task<Booking?> GetBookingByQrAsync(string qrCode)
        {
            return await _db.Bookings
                .Include(b => b.Event)
                    .ThenInclude(e => e.Venue)
                .FirstOrDefaultAsync(b => b.QrCode == qrCode);
        }


        public async Task<ManagerRevenueDto> GetEventStatsAsync(int eventId)
        {
            return await _db.Bookings
                .Where(b => b.EventId == eventId &&
                            b.PaymentStatus != PaymentStatus.Cancelled)
                .Select(b => new
                {
                    b.TicketCount,
                    TicketPrice = b.Event!.TicketPrice
                })
                .GroupBy(_ => 1)
                .Select(g => new ManagerRevenueDto
                {
                    TicketsSold = g.Sum(x => x.TicketCount),
                    Revenue = g.Sum(x => x.TicketCount * x.TicketPrice),
                    BookingCount = g.Count()
                })
                .FirstOrDefaultAsync()
                ?? new ManagerRevenueDto();
        }
        public async Task<PagedResult<BookingDto>> GetBookingsForAdminAsync(PagedRequest request)
        {
            var query = _db.Bookings
                .AsNoTracking()
                .Include(b => b.Event)
                    .ThenInclude(e => e.Venue)
                .Include(b => b.Event)
                    .ThenInclude(e => e.EventCategory)
                .Where(b =>
                    b.Event != null &&
                    b.Event.ManagerId == null &&              
                    b.PaymentStatus == PaymentStatus.Paid
                )
                .AsQueryable();

            //  Search
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.Where(b =>
                    b.Event!.Title.Contains(request.Search));
            }

            //  Date filter
            if (request.DateFilter.HasValue)
            {
                query = query.Where(b =>
                    b.Event!.StartDate.Date == request.DateFilter.Value.Date);
            }
            if (request.CategoryId.HasValue)
            {
                query = query.Where(b =>
                    b.Event!.EventCategoryId == request.CategoryId);
            }


            //  Total distinct events
            var totalCount = await query
                .GroupBy(b => b.EventId)
                .CountAsync();

            //  Grouped result (per event)
            var items = await query
                .GroupBy(b => new
                {
                    b.EventId,
                    b.Event.Title,
                    b.Event.StartDate,
                    b.Event.EndDate,
                    b.Event.TicketPrice,
                    TotalTickets = b.Event.TotalTickets,
                    VenueName = b.Event.Venue!.VenueName,
                    CategoryName = b.Event.EventCategory!.Name
                })
                .Select(g => new BookingDto
                {
                    EventId = g.Key.EventId,
                    EventName = g.Key.Title,
                    CategoryName = g.Key.CategoryName,
                    EventStartDate = g.Key.StartDate,
                    EventEndDate = g.Key.EndDate,
                    VenueName = g.Key.VenueName,
                    TicketPrice = g.Key.TicketPrice,
                    TicketCount = g.Sum(x => x.TicketCount), // SOLD
                    TotalTickets = g.Key.TotalTickets,        // CAPACITY
                    ManagerName = "Admin"
                })
                .OrderByDescending(x => x.EventStartDate)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new PagedResult<BookingDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }


        public async Task<PagedResult<BookingDto>> GetBookingsByManagerIdAsync(int managerId,PagedRequest request)
        {
            var query = _db.Bookings
                .AsNoTracking()
                .Include(b => b.Event)
                    .ThenInclude(e => e.Venue)
                .Include(b => b.Event)
                    .ThenInclude(e => e.EventCategory)
                .Where(b =>
                    b.Event != null &&
                    b.Event.ManagerId == managerId &&
                    b.PaymentStatus == PaymentStatus.Paid
                )
                .AsQueryable();

            //  Search
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.Where(b =>
                    b.Event!.Title.Contains(request.Search));
            }

            if (request.CategoryId.HasValue)
            {
                query = query.Where(b =>
                    b.Event!.EventCategoryId == request.CategoryId);
            }

            //  Date filter
            if (request.DateFilter.HasValue)
            {
                query = query.Where(b =>
                    b.Event!.StartDate.Date == request.DateFilter.Value.Date);
            }

            //  Total Count
            var totalCount = await query
                .GroupBy(b => b.EventId)
                .CountAsync();

            //  Pagination 
            var items = await query
                .GroupBy(b => new
                {
                    b.EventId,
                    b.Event.Title,
                    b.Event.StartDate,
                    b.Event.EndDate,
                    b.Event.TicketPrice,
                    TotalTickets=b.Event.TotalTickets,
                    VenueName = b.Event.Venue!.VenueName,
                    CategoryName = b.Event.EventCategory!.Name
                })
                .Select(g => new BookingDto
                {
                    EventId = g.Key.EventId,
                    EventName = g.Key.Title,
                    CategoryName = g.Key.CategoryName,
                    EventStartDate = g.Key.StartDate,
                    EventEndDate = g.Key.EndDate,
                    VenueName = g.Key.VenueName,
                    TicketPrice = g.Key.TicketPrice,
                    TicketCount = g.Sum(x => x.TicketCount),
                    TotalTickets = g.Key.TotalTickets

                })
                .OrderByDescending(x => x.EventStartDate)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new PagedResult<BookingDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }



    }
}
