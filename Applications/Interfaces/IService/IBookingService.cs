using Applications.Dto;

namespace Applications.Interfaces.IService
{
    public interface IBookingService
    {

        Task<BookingDto> CreateBookingAsync(BookingRequest request ,int userId);
        Task<IEnumerable<BookingDto>> GetAllBookingAsync();
        Task<IEnumerable<BookingDto>> GetBookingByUserAsync(int userId);
        Task CancelBookingAsync(int bookingId);
    }
}
