using Applications.Dto;
using Applications.Interfaces.IRepository;
using Applications.Interfaces.IService;
using Domains.Entities;

namespace EventBooking_TicketManagement_API.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;

        public EventService(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public async Task<IEnumerable<EventDto>> GetAllEventsAsync()
        {
            var events = await _eventRepository.GetAllAsync();

            return events.Select(ev => new EventDto
            {
                //Id = ev.Id,
                //Title = ev.Title,
                //EventType = ev.EventType,
                //Description = ev.Description,
                //Genre = ev.Genre,
                //Language = ev.Language,
                //Duration = ev.Duration,
                //ShowDate = ev.ShowDate,
                ////VenueId = ev.VenueId,
                //VenueName = ev.Venue?.VenueName ?? string.Empty,
                //CityName = ev.Venue?.City?.CityName ?? string.Empty,
                //TicketPrice = ev.TicketPrice,
                //ImageUrl = ev.ImageUrl
                Id = ev.Id,
                Title = ev.Title,
                EventType = ev.EventType,
                Description = ev.Description,
                Genre = ev.Genre,
                Language = ev.Language,
                Duration = ev.Duration,
                ShowDate = ev.ShowDate,
                VenueName = ev.Venue?.VenueName ?? string.Empty,
                CityName = ev.Venue?.City?.CityName ?? string.Empty,
                TicketPrice = ev.TicketPrice,
                ImageUrl = ev.ImageUrl,
                Status = ev.Status,
                ManagerName = ev.ManagerName
            });
        }

        public async Task<EventDto?> GetEventByIdAsync(int id)
        {
            var ev = await _eventRepository.GetByIdAsync(id);
            if (ev == null) return null;

            return new EventDto
            {
                //Id = ev.Id,
                //Title = ev.Title,
                //EventType = ev.EventType,
                //Description = ev.Description,
                //Genre = ev.Genre,
                //Language = ev.Language,
                //Duration = ev.Duration,
                //ShowDate = ev.ShowDate,
                //VenueId = ev.VenueId,
                //VenueName = ev.Venue?.VenueName ?? string.Empty,
                //CityName = ev.Venue?.City?.CityName ?? string.Empty,
                //TicketPrice = ev.TicketPrice,
                //ImageUrl = ev.ImageUrl

                Id = ev.Id,
                Title = ev.Title,
                EventType = ev.EventType,
                Description = ev.Description,
                Genre = ev.Genre,
                Language = ev.Language,
                Duration = ev.Duration,
                ShowDate = ev.ShowDate,
                VenueId = ev.VenueId,
                VenueName = ev.Venue?.VenueName ?? string.Empty,
                CityName = ev.Venue?.City?.CityName ?? string.Empty,
                TicketPrice = ev.TicketPrice,
                ImageUrl = ev.ImageUrl,
                Status = ev.Status,
                ManagerName = ev.ManagerName,
                AdminNote = ev.AdminNote
            };
        }

        public async Task AddEventAsync(EventDto dto)
        {
            var ev = new Event
            {
                Title = dto.Title,
                EventType = dto.EventType,
                Description = dto.Description,
                Genre = dto.Genre,
                Language = dto.Language,
                Duration = dto.Duration,
                ShowDate = dto.ShowDate,
                VenueId = dto.VenueId,
                TicketPrice = dto.TicketPrice,
                ImageUrl = dto.ImageUrl,

                Status = EventStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _eventRepository.AddAsync(ev);
        }

        public async Task UpdateEventAsync(int id, EventDto dto)
        {
            var ev = await _eventRepository.GetByIdAsync(id);
            if (ev == null) throw new KeyNotFoundException($"Event with Id {id} not found.");

            ev.Title = dto.Title;
            ev.EventType = dto.EventType;
            ev.Description = dto.Description;
            ev.Genre = dto.Genre;
            ev.Language = dto.Language;
            ev.Duration = dto.Duration;
            ev.ShowDate = dto.ShowDate;
            //ev.VenueId = dto.VenueId;
            ev.TicketPrice = dto.TicketPrice;
            ev.ImageUrl = dto.ImageUrl;

            await _eventRepository.UpdateAsync(ev);
        }

        public async Task DeleteEventAsync(int id)
        {
            await _eventRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<SeatDto>> GetSeatsByEventIdAsync(int eventId)
        {
            var ev = await _eventRepository.GetByIdAsync(eventId);
            if (ev == null || ev.Seats == null) return Enumerable.Empty<SeatDto>();

            return ev.Seats.Select(s => new SeatDto
            {
                Id = s.Id,
                SeatNumber = s.SeatNumber,
                Category = s.Category,
                IsBooked = s.IsBooked,
                EventId = s.EventId
            });
        }

        public async Task<IEnumerable<EventDto>> GetPendingEventsAsync()
        {
            var events = await _eventRepository.GetPendingEventsAsync();
            return events.Select(MapToDto);
        }

        public async Task ApproveEventAsync(int eventId)
        {
            var ev = await _eventRepository.GetByIdAsync(eventId)
                ?? throw new Exception("Event not found");

            ev.Status = EventStatus.Approved;
            ev.ApprovedAt = DateTime.UtcNow;
            await _eventRepository.UpdateAsync(ev);
        }

        public async Task RejectEventAsync(int eventId, EventRejectDto dto)
        {
            var ev = await _eventRepository.GetByIdAsync(eventId)
                ?? throw new Exception("Event not found");

            ev.Status = EventStatus.Rejected;
            ev.AdminNote = dto.Reason;
            await _eventRepository.UpdateAsync(ev);
        }

        // Approved events
        public async Task<IEnumerable<EventDto>> GetApprovedEventsAsync()
        {
            var events = await _eventRepository.GetApprovedEventsAsync();
            return events.Select(MapToDto);
        }

        //  Helper Mapper
        private static EventDto MapToDto(Event e) => new()
        {
            Id = e.Id,
            Title = e.Title,
            EventType = e.EventType,
            Description = e.Description,
            Genre = e.Genre,
            Language = e.Language,
            Duration = e.Duration,
            ShowDate = e.ShowDate,
            TicketPrice = e.TicketPrice,
            ImageUrl = e.ImageUrl,
            VenueName = e.Venue?.VenueName ?? string.Empty,
            ManagerName = e.ManagerName,
            Status = e.Status,
            AdminNote = e.AdminNote,
            CreatedAt = e.CreatedAt,
            ApprovedAt = e.ApprovedAt
        };

        public async Task<EventDto> CreateEventAsync(ManagerEventDto mdto, string managerId, string managerName)
        {
            var ev = new Event
            {
                Title = mdto.Title,
                EventType = mdto.EventType,
                Description = mdto.Description,
                Genre = mdto.Genre,
                Language = mdto.Language,
                Duration = mdto.Duration,
                ShowDate = mdto.ShowDate,
                TicketPrice = mdto.TicketPrice,
                ImageUrl = "",
                VenueId = mdto.VenueId,
                ManagerId = managerId,
                ManagerName = managerName,
                Status = EventStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _eventRepository.AddAsync(ev);
            return new EventDto
            {
                Id = ev.Id,
                Title = ev.Title,
                EventType = ev.EventType,
                Description = ev.Description,
                Genre = ev.Genre,
                Language = ev.Language,
                Duration = ev.Duration,
                ShowDate = ev.ShowDate,
                TicketPrice = ev.TicketPrice,
                ManagerId = ev.ManagerId,
                ManagerName = ev.ManagerName,
                Status = ev.Status,
                CreatedAt = ev.CreatedAt,

            };
        }

        public async Task<IEnumerable<EventDto>> GetManagerEventsAsync(string managerId)
        {
            var events = await _eventRepository.GetEventsByManagerByIdAsync(managerId);
            return events.Select(e => new EventDto
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                VenueName = e.Venue?.VenueName ?? string.Empty,
                Status = e.Status,
                CreatedAt = e.CreatedAt,
                ShowDate = e.ShowDate
            });
        }
    }
}

