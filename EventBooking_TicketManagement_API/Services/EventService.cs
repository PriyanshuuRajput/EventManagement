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
                Id = ev.Id,
                Title = ev.Title,
                EventType = ev.EventType,
                Description = ev.Description,
                Genre = ev.Genre,
                Language = ev.Language,
                Duration = ev.Duration,
                ShowDate = ev.ShowDate,
                //VenueId = ev.VenueId,
                VenueName = ev.Venue?.VenueName ?? string.Empty,
                CityName = ev.Venue?.City?.CityName ?? string.Empty,
                TicketPrice = ev.TicketPrice,
                ImageUrl = ev.ImageUrl
            });
        }

        public async Task<EventDto?> GetEventByIdAsync(int id)
        {
            var ev = await _eventRepository.GetByIdAsync(id);
            if (ev == null) return null;

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
                VenueId = ev.VenueId,
                VenueName = ev.Venue?.VenueName ?? string.Empty,
                CityName = ev.Venue?.City?.CityName ?? string.Empty,
                TicketPrice = ev.TicketPrice,
                ImageUrl = ev.ImageUrl
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
                ImageUrl = dto.ImageUrl
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
    }
}
