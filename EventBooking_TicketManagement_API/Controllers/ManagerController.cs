using Applications.Dto;
using Applications.Interfaces.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace EventBooking_TicketManagement_API.Controllers
{
    //[Authorize(Roles = "Organizer")] // ✅ Restrict only to Managers/Organizers
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ManagerEventController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ManagerEventController(IEventService eventService, IWebHostEnvironment hostEnvironment)
        {
            _eventService = eventService;
            _hostEnvironment = hostEnvironment;
        }

        // Create new event 
        //[HttpPost("create")]
        //[Consumes("multipart/form-data")]
        //public async Task<IActionResult> CreateEvent([FromForm] ManagerEventDto dto)
        //{
        //    // Extract manager identity from JWT
        //    var managerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        //    if (!int.TryParse(managerIdClaim, out var managerId))
        //        return BadRequest("Invalid ManagerId in JWT token.");

        //    var managerName = User.Identity?.Name ?? "Unknown Manager";


        //    if (dto.ImageFile != null)
        //    {
        //        var allowedExtensions = new[] { ".jpeg", ".jpg", ".png", ".webp" };
        //        var ext = Path.GetExtension(dto.ImageFile.FileName).ToLowerInvariant();

        //        if (!allowedExtensions.Contains(ext))
        //            return BadRequest("Invalid image file type.");
        //        var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "Uploads");


        //        if (!Directory.Exists(uploadsFolder))
        //            Directory.CreateDirectory(uploadsFolder);

        //        var uniqueFile = $"{Guid.NewGuid()}{ext}";
        //        var filePath = Path.Combine(uploadsFolder, uniqueFile);

        //        using var fileStream = new FileStream(filePath, FileMode.Create);
        //        await dto.ImageFile.CopyToAsync(fileStream);

        //        //  Generate public URL for image
        //        dto.ImageUrl = $"{Request.Scheme}://{Request.Host}/Uploads/{uniqueFile}";

        //    }

        //    var createdEvent = await _eventService.CreateEventAsync(dto, managerId, managerName);

        //    return Ok(new
        //    {
        //        Message = " Event submitted successfully! Pending admin approval.",
        //        Data = createdEvent
        //    });
        //}
        [HttpPost("create")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateEvent([FromForm] ManagerEventDto dto)
        {
            // Extract manager identity FROM JWT (and parse to int)
            var managerIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(managerIdClaim, out var managerId))
                return BadRequest("Invalid ManagerId in JWT token.");

            var managerName = User.Identity?.Name ?? "Unknown Manager";

            // 1️ Handle image file
            if (dto.ImageFile != null)
            {
                var allowedExt = new[] { ".jpeg", ".jpg", ".png", ".webp" };
                var ext = Path.GetExtension(dto.ImageFile.FileName).ToLowerInvariant();

                if (!allowedExt.Contains(ext))
                    return BadRequest("Invalid image file type.");

                var uploadFolder = Path.Combine(_hostEnvironment.WebRootPath, "Uploads");
                if (!Directory.Exists(uploadFolder))
                    Directory.CreateDirectory(uploadFolder);

                var fileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploadFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.ImageFile.CopyToAsync(stream);
                }

                dto.ImageUrl = $"{Request.Scheme}://{Request.Host}/Uploads/{fileName}";
            }

            // 2 Create event
            var createdEvent = await _eventService.CreateEventAsync(dto, managerId, managerName);

            return Ok(new
            {
                Message = "Event submitted successfully! Pending admin approval.",
                Data = createdEvent
            });
        }


        //  Get all events created by this manager
        [HttpGet("my-events")]
        public async Task<IActionResult> GetMyEvents()
        {
            var managerIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(managerIdClaim, out var managerId))
                return BadRequest("Invalid ManagerId in JWT token.");

            var events = await _eventService.GetManagerEventsAsync(managerId);
            return Ok(events);
        }


        // (Optional) Update event before approval
        [HttpPut("update/{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateMyEvent(int id, [FromForm] ManagerEventDto dto)
        {
            var existingEvent = await _eventService.GetEventByIdAsync(id);
            if (existingEvent == null)
                return NotFound($"Event with ID {id} not found.");

            //  Handle image upload 
            if (dto.ImageFile != null)
            {
                var allowedExtensions = new[] { ".jpeg", ".jpg", ".png", ".webp" };
                var ext = Path.GetExtension(dto.ImageFile.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(ext))
                    return BadRequest("Invalid image file type.");

                var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "Uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFile = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploadsFolder, uniqueFile);

                using var fileStream = new FileStream(filePath, FileMode.Create);
                await dto.ImageFile.CopyToAsync(fileStream);

                dto.ImageUrl = $"{Request.Scheme}://{Request.Host}/Uploads/{uniqueFile}";
            }
            else
            {
                // If no new image uploaded, keep existing one
                dto.ImageUrl = existingEvent.ImageUrl;
            }
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
                //TicketPrice = dto.TicketPrice,
                ImageUrl = dto.ImageUrl,


                ManagerId = existingEvent.ManagerId,
                ManagerName = existingEvent.ManagerName,
                Status = existingEvent.Status,
                AdminNote = existingEvent.AdminNote,
                TotalTickets = existingEvent.TotalTickets,
                SoldTickets = existingEvent.SoldTickets,
                CreatedAt = existingEvent.CreatedAt,
                ApprovedAt = existingEvent.ApprovedAt
            });

            return Ok("Event updated successfully.");
        }

        [HttpPost("pay/{eventId}")]
        public async Task<IActionResult> PayEventAmount(int eventId)
        {
            try
            {
                await _eventService.MarkEventAsPaidAsync(eventId);
                return Ok(new { Message = "Payment successful! Event is now published." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }


        // (Optional) Delete event before approval
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            await _eventService.DeleteEventAsync(id);
            return Ok("Event deleted successfully.");
        }


    }
}
