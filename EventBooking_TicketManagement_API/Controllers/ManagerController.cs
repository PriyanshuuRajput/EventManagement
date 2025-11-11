using Applications.Dto;
using Applications.Interfaces.IService;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EventBooking_TicketManagement_API.Controllers
{
    //[Authorize(Roles = "Manager")]
    [ApiController]
    [Route("api/[controller]")]
    public class ManagerEventController : ControllerBase
    {
        private readonly IEventService _eventService;

        public ManagerEventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        // Create a new event (Status = Pending)
        [HttpPost("create")]
        public async Task<IActionResult> CreateEvent([FromBody] EventDto dto)
        {
            var managerId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "unknown";
            var managerName = User.Identity?.Name ?? "Unknown Manager";

            var createdEvent = await _eventService.CreateEventAsync(dto, managerId, managerName);
            return Ok(new { Message = "Event submitted for admin approval.", Data = createdEvent });
        }

        //  View all events created by this manager
        [HttpGet("my-events")]
        public async Task<IActionResult> GetMyEvents()
        {
            var managerId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "unknown";
            var events = await _eventService.GetAllEventsAsync();
            return Ok(events);
        }

        //// (Optional) Update your event before approval
        //[HttpPut("update/{id}")]
        //public async Task<IActionResult> UpdateMyEvent(int id, [FromBody] EventDto dto)
        //{
        //    await _eventService.UpdateEventAsync(id, dto);
        //    return Ok("Event updated successfully.");
        //}

        // ✅ (Optional) Delete event before approval
        //[HttpDelete("delete/{id}")]
        //public async Task<IActionResult> DeleteEvent(int id)
        //{
        //    await _eventService.DeleteEventAsync(id);
        //    return Ok("Event deleted successfully.");
        //}
    }
}
