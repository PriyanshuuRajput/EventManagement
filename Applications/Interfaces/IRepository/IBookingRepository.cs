using Applications.Dto;
using Domains.Entities;

public interface IBookingRepository
{
    Task<Booking> CreateBookingAsync(Booking booking);
    Task<IEnumerable<Booking>> GetAllBookingAsync();
    Task<Booking?> GetBookingByIdAsync(int id);
    Task<IEnumerable<Booking>> GetBookingsByUserAsync(int userId);
    Task<int> GetActiveTicketCountByEventAsync(int eventId);
    Task<int> GetUserTicketCountByEventAsync(int eventId, int userId);
    Task UpdateAsync(Booking booking);
    Task <AdminUser?> GetUserByIdAsync(int userId);
    Task<Booking?> GetBookingByQrAsync(string qrCode);
    Task<ManagerRevenueDto?> GetEventStatsAsync(int eventId);


}
