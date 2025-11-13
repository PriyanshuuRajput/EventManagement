using Applications.Dto;
using Applications.Interfaces.IRepository;
using Applications.Interfaces.IService;
using Domains.Entities;

namespace EventBooking_TicketManagement_API.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IEmailService _emailService;

        public EventService(IEventRepository eventRepository, IEmailService emailService)
        {
            _eventRepository = eventRepository;
            _emailService = emailService;
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
                AdminNote = ev.AdminNote,
                OfferedEventAmount = ev.OfferedEventAmount,
                EventAmount = ev.EventAmount,
                IsAmountAccepted = ev.IsPrizePaid

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

            if (dto.VenueId != 0)
                ev.VenueId = dto.VenueId;

            ev.TicketPrice = dto.TicketPrice;

            if (!string.IsNullOrEmpty(dto.ImageUrl))
                ev.ImageUrl = dto.ImageUrl;
            ev.ManagerId = dto.ManagerId;
            ev.ManagerName = dto.ManagerName;
            ev.Status = dto.Status;
            ev.AdminNote = dto.AdminNote;
            ev.TicketPrice = dto.TicketPrice;

            //ev.SoldTicket = dto.SoldTickets;
            //ev.CreatedAt = dto.CreatedAt;
            ev.ApprovedAt = dto.ApprovedAt;

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
                //TicketPrice = mdto.TicketPrice,
                ImageUrl = mdto.ImageUrl!,
                VenueId = mdto.VenueId,
                ManagerId = managerId,
                ManagerName = managerName,
                Status = EventStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                OfferedEventAmount = mdto.OfferedEventAmount ?? 0m,
                IsPrizePaid = false,
                PrizePaidAt = null,
                EventAmount = 0m,
            };

            await _eventRepository.AddAsync(ev);

            //Send Email to Admin
            string adminEmail = "rajputronak0058@gmail.com";
            string subject = $"New Event Submitted :{ev.Title}";

            string approvalUrl = $"https://localhost:7117/approval-manager-events/{ev.Id}";
            string body = $@"
                            <h2>New Event pending Approval </h2>
                            <p><b>Title:</b> {ev.Title}</p>
                            <p><b>Manager:</b> {managerName}</p>
                            <p><b>Date:</b> {ev.ShowDate:dd MMM yyyy HH:mm}</p>
                            <p>Please review and approve/reject this event in the admin dashboard.</p>
                            <p>
                                <a href ='{approvalUrl}' style ='color:#fff; background-color:#d9534f;padding:10px 15px ; border-radius:6px; text-decoration:none;'>Review Event</a>
                            </p>
                            <hr/>
                            <small>This Link opens the Admin Dashboard for event verification</small>";

            await _emailService.SendEmailAsync(adminEmail, subject, body);

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

        public async Task<IEnumerable<EventDto>> GetPendingEventsAsync()
        {
            var events = await _eventRepository.GetPendingEventsAsync();
            return events.Select(MapToDto);
        }

        public async Task ApproveEventAsync(int eventId)
        {
            var ev = await _eventRepository.GetByIdAsync(eventId)
                ?? throw new Exception("Event not found");

            ev.Status = EventStatus.AdminApproved;
            ev.ApprovedAt = DateTime.UtcNow;
            ev.AdminNote = null;
            ev.IsPrizePaid = false;
            ev.PrizePaidAt = null;
            await _eventRepository.UpdateAsync(ev);

            //Send email to Manager

            string managerEmail = "rajputpriyanshu676@gmail.com";
            string subject = $"Event Approved :{ev.Title}";
            string body = $@"<h2>Good news!</h2>
                          <p>Dear {ev.ManagerName},</p>
                          <p>Your event <strong>{ev.Title}</strong> has been approved by the admin.</p>
                          <p>🎉 It is now visible to all users in the application.</p>";

            await _emailService.SendEmailAsync(managerEmail, subject, body);
        }




        public async Task RejectEventAsync(int eventId, EventRejectDto dto)
        {
            var ev = await _eventRepository.GetByIdAsync(eventId)
                ?? throw new Exception("Event not found");

            ev.Status = EventStatus.Rejected;
            ev.AdminNote = dto.Reason;
            await _eventRepository.UpdateAsync(ev);

            string managerEmail = "rajputpriyanshu676@gmail.com";
            string subject = $"Event Rejected:{ev.Title}";
            string body = $@"<h2>Your Event Was Rejected</h2>
        <p>Dear {ev.ManagerName},</p>
        <p>Unfortunately, your event <b>{ev.Title}</b> was rejected by the admin.</p>
        <p><b>Reason:</b> {dto.Reason}</p>
        <p>You can modify and resubmit the event for approval.</p>";

            await _emailService.SendEmailAsync(managerEmail, subject, body);
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
            ApprovedAt = e.ApprovedAt,
            OfferedEventAmount = e.OfferedEventAmount,
            EventAmount = e.EventAmount,
            IsAmountAccepted = e.IsPrizePaid


        };


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


        public async Task AcceptOfferedAmountAsync(int eventId)
        {
            var ev = await _eventRepository.GetByIdAsync(eventId)
                ?? throw new Exception("Event not found");

            if (!ev.OfferedEventAmount.HasValue || ev.OfferedEventAmount.Value <= 0)
                throw new Exception("No amount was offered by manager.");


            // copy manager's offer into admin-controlled EventAmount
            ev.EventAmount = ev.OfferedEventAmount.Value;
            ev.Status = EventStatus.AdminApproved; // offer accepted
            ev.IsPrizePaid = false;
            ev.PrizePaidAt = null;
            await _eventRepository.UpdateAsync(ev);

            // Send Email to Manager
            string managerEmail = "rajputpriyanshu676@gmail.com";
            string subject = $"Offer Accepted for Event: {ev.Title}";
            string body = $@"<h2>Your offer has been accepted</h2>
                     <p>Please pay the final amount: <b>₹{ev.OfferedEventAmount}</b></p>";

            await _emailService.SendEmailAsync(managerEmail, subject, body);
        }


        public async Task MarkEventAsPaidAsync(int eventId)
        {
            var ev = await _eventRepository.GetByIdAsync(eventId)
                ?? throw new Exception("Event not found");

            if (ev.IsPrizePaid)
                throw new Exception("Payment already done.");

            ev.IsPrizePaid = true;
            ev.PrizePaidAt = DateTime.UtcNow;
            ev.Status = EventStatus.PaymentConfirmed;

            await _eventRepository.UpdateAsync(ev);

            // Notify Admin
            string adminEmail = "rajputronak0058@gmail.com";
            string subject = $"Event Payment Completed: {ev.Title}";
            string body = $@"<h3>Payment Completed</h3>
                     <p>Manager has paid the event amount.</p>
                     <p><b>Amount Paid:</b> ₹{ev.EventAmount}</p>";

            await _emailService.SendEmailAsync(adminEmail, subject, body);
        }


    }
}

