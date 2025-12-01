using Applications.Dto;
using Applications.Interfaces.IService;
using Microsoft.AspNetCore.Mvc;

namespace EventBooking_TicketManagement_API.Controllers
{
    //[Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly IManagerService _managerService;

        public AdminController(IEventService eventService, IManagerService managerService)
        {
            _eventService = eventService;
            _managerService = managerService;
        }

        [HttpGet("managers")]
        public async Task<IActionResult> GetAllManagers()
        {
            var result = await _managerService.GetAllManagersAsync();
            return Ok(result);
        }


        [HttpGet("pending-managers")]
        public async Task<IActionResult> GetPendingManagers()
        {
            var pending = await _managerService.GetPendingManagersAsync();
            return Ok(pending);
        }

        [HttpPost("approve-manager/{id}")]
        public async Task<IActionResult> ApproveManager(int id)
        {
            var result = await _managerService.ApproveManagerAsync(id);

            if (result.Contains("not found") || result.Contains("Already"))
                return BadRequest(new { message = result });

            return Ok(new { message = result });
        }

        [HttpPost("reject-manager/{id}")]
        public async Task<IActionResult> RejectManager(int id, [FromBody] EventRejectDto dto)
        {
            await _managerService.RejectManagerAsync(id, dto.Reason);
            return Ok(new { message = "Manager request rejected." });
        }
        //Delete Managers

        [HttpDelete("delete-manager/{id}")]
        public async Task<IActionResult> DeleteManager(int id)
        {
            var result = await _managerService.DeleteManagerAsync(id);
            if (!result)
            {
                return NotFound(new { message = "Manager not found!" });
            }
            return Ok(new { message = "Manager delete successfully!" });

        }


        [HttpGet("events-by-manager/{managerId}")]
        public async Task<IActionResult> GetEventsByManager(int managerId)
        {
            var events = await _eventService.GetManagerEventsAsync(managerId);

            return Ok(events);
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


        [HttpPost("accept-offer/{id}")]
        public async Task<IActionResult> AcceptOffer(int id, [FromBody] AcceptOfferDto dto)
        {
            try
            {
                await _eventService.AcceptOfferedAmountAsync(id, dto.FinalAmount);
                return Ok(new { Message = "Offer accepted. Waiting for manager payment." });
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Error = "Something went Wrong." });
            }
        }

    }
}
