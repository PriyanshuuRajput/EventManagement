using Applications.Dto;
using Applications.Dto.Pagination;

namespace Applications.Interfaces.IService
{
    public interface IEventService
    {
        Task<IEnumerable<EventDto>> GetAllEventsAsync();
        Task<EventDto?> GetEventByIdAsync(int id);
        Task AddEventAsync(EventDto dto);
        Task UpdateEventAsync(int id, EventDto dto);
        Task DeleteEventAsync(int id);
        Task<IEnumerable<SeatDto>> GetSeatsByEventIdAsync(int eventId);
        Task<EventDto> CreateEventAsync(ManagerEventDto mdto, int managerId, string managerName);

        Task<IEnumerable<EventDto>> GetManagerEventsAsync(int managerId);

        //  Admin
        Task<IEnumerable<EventDto>> GetPendingEventsAsync();
        Task ApproveEventAsync(int eventId);
        Task RejectEventAsync(int eventId, EventRejectDto dto);

        //  User
        Task<IEnumerable<EventDto>> GetApprovedEventsAsync();

        Task AcceptOfferedAmountAsync(int eventId, decimal finalAmount);
        Task MarkEventAsPaidAsync(int eventId);


        //PAgination
        Task<PagedResult<EventDto>> GetPagedEventAsync(PagedRequest req);


    }
}
