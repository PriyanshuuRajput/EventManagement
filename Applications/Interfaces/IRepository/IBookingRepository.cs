using Domains.Entities;

public interface IBookingRepository
{
    Task<Booking> CreateBookingAsync(Booking booking);
    Task<IEnumerable<Booking>> GetAllBookingAsync();
    Task<Booking?> GetBookingByIdAsync(int id);
    Task<IEnumerable<Booking>> GetBookingsByUserAsync(int userId);
    Task UpdateAsync(Booking booking);
}
