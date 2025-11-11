using Domains.Entities;

namespace Applications.Interfaces.IRepository
{
    public interface IBookingRepository
    {
        Task<Booking> CreateBookingAsync(Booking booking);
        Task<Booking?> GetBookingByIdAsync(int id);
        Task<IEnumerable<Booking>> GetAllBookingAsync();
        Task<IEnumerable<Booking>> GetBookingsByUserAsync(string userEmail);
        Task<bool> DeleteBookingAsync(int id);
        Task ReleaseSeatsAsync(int bookingId);
    }
}
