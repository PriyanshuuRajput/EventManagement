using Applications.Dto;

namespace Applications.Interfaces.IService
{
    public interface IBookingService
    {
        Task CancelBookingAsync(int bookingId);
        Task<BookingDto> CreateBookingAsync(BookingRequest request);
        Task<IEnumerable<BookingDto>> GetAllBookingAsync();
        Task<IEnumerable<BookingDto>> GetBookingByUserAsync(string userEmail);
    }
}
