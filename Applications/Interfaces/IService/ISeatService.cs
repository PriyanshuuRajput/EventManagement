using Applications.Dto;

namespace Applications.Interfaces.IService
{
    public interface ISeatService
    {
        // Get all seats for a specific event (for user seat selection)
        //Task<IEnumerable<SeatDto>> GetSeatsByEventAsync(int eventId);

        //// Get all seats (for admin management)
        //Task<IEnumerable<SeatDto>> GetAllSeatsAsync();

        // Admin: Add multiple seats to an event
        Task AddSeatsAsync(int eventId, int totalSeats, string category, decimal price);

        //Update seat booking status (when user books/cancels)
        Task UpdateSeatBookingAsync(int seatId, bool isBooked, int? bookingId = null);

        // Get seat summary for UI (total/booked/available)
        Task<(int total, int booked, int available)> GetSeatSummaryAsync(int eventId);
    }
}
