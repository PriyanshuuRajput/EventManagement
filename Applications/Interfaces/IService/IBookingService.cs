using Applications.Dto;
using Applications.Dto.Pagination;

namespace Applications.Interfaces.IService
{
    public interface 
        IBookingService
    {

        Task<BookingDto> CreateBookingAsync(BookingRequest request ,int userId);
        Task<IEnumerable<BookingDto>> GetAllBookingAsync();
        Task<IEnumerable<BookingDto>> GetBookingByUserAsync(int userId);
        Task<PagedResult<BookingDto>> GetBookingsByManagerAsync(int managerId,PagedRequest request);
        Task<PagedResult<BookingDto>> GetAllBookingsForAdminAsync(PagedRequest request);
        Task CancelBookingAsync(int bookingId);
        
    }
}
