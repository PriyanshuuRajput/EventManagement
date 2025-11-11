using Applications.Dto;
using Applications.Interfaces.IService;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EventBooking_TicketManagement_API.Controllers
{
    //[Authorize(Roles = "Organizer")] // ✅ Restrict only to Managers/Organizers
    [ApiController]
    [Route("api/[controller]")]
    public class ManagerEventController : ControllerBase
    {
        private readonly IEventService _eventService;

        public ManagerEventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        // ✅ Create new event (goes to admin as Pending)
        [HttpPost("create")]
        [Consumes("multipart/form-data")] // ✅ Required for file upload
        public async Task<IActionResult> CreateEvent([FromForm] ManagerEventDto dto)
        {
            // Extract manager identity from JWT
            var managerId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "unknown";
            var managerName = User.Identity?.Name ?? "Unknown Manager";

            var createdEvent = await _eventService.CreateEventAsync(dto, managerId, managerName);

            return Ok(new
            {
                Message = "✅ Event submitted successfully! Pending admin approval.",
                Data = createdEvent
            });
        }

        // ✅ Get all events created by this manager
        [HttpGet("my-events")]
        public async Task<IActionResult> GetMyEvents()
        {
            var managerId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "unknown";
            var events = await _eventService.GetManagerEventsAsync(managerId);

            return Ok(events);
        }

        // ✅ (Optional) Update event before approval
        [HttpPut("update/{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateMyEvent(int id, [FromForm] ManagerEventDto dto)
        {
            await _eventService.UpdateEventAsync(id, new EventDto
            {
                Id = id,
                Title = dto.Title,
                EventType = dto.EventType,
                Description = dto.Description,
                Genre = dto.Genre,
                Language = dto.Language,
                Duration = dto.Duration,
                ShowDate = dto.ShowDate,
                VenueId = dto.VenueId,
                TicketPrice = dto.TicketPrice,
                ImageFile = dto.ImageFile
            });

            return Ok("Event updated successfully.");
        }

        // ✅ (Optional) Delete event before approval
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            await _eventService.DeleteEventAsync(id);
            return Ok("Event deleted successfully.");
        }
    }
}
