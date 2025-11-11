using Applications.Dto;
using Applications.Interfaces.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventBooking_TicketManagement_API.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminEventController : ControllerBase
    {
        private readonly IEventService _eventService;

        public AdminEventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        //  Get all pending events
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingEvents()
        {
            var events = await _eventService.GetPendingEventsAsync();
            return Ok(events);
        }

        //  Approve event
        [HttpPost("approve/{id}")]
        public async Task<IActionResult> ApproveEvent(int id)
        {
            await _eventService.ApproveEventAsync(id);
            return Ok(new { Message = "Event approved successfully." });
        }

        //  Reject event with reason
        [HttpPost("reject/{id}")]
        public async Task<IActionResult> RejectEvent(int id, [FromBody] EventRejectDto dto)
        {
            await _eventService.RejectEventAsync(id, dto);
            return Ok(new { Message = "Event rejected successfully." });
        }

        // View rejected events
        //[HttpGet("rejected")]
        //public async Task<IActionResult> GetRejectedEvents()
        //{
        //    var events = await _eventService.RejectEventAsync();
        //    return Ok(events);
        //}
    }
}
