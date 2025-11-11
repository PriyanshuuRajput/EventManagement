using Applications.Dto;

namespace Applications.Interfaces.IService
{
    public interface IEventService
    {
        Task<IEnumerable<EventDto>> GetAllEventsAsync();
        Task<EventDto?> GetEventByIdAsync(int id);         // ✅ Guid
        Task AddEventAsync(EventDto dto);
        Task UpdateEventAsync(int id, EventDto dto);       // ✅ Guid + same params as service
        Task DeleteEventAsync(int id);                     // ✅ Guid
        Task<IEnumerable<SeatDto>> GetSeatsByEventIdAsync(int eventId); // ✅ Guid
    }
}
