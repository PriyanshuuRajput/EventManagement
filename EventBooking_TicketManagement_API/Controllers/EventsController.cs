using Applications.Dto;
using Applications.Dto.Pagination;
using Applications.Interfaces.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EventBooking_TicketManagement_API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly IWebHostEnvironment _hostEnvironment;

        public EventsController(IEventService eventService, IWebHostEnvironment hostEnvironment)
        {
            _eventService = eventService;
            _hostEnvironment = hostEnvironment;
        }

        // GET: api/events
        [HttpGet]
        public async Task<IActionResult> GetAllEvents()
        {
            var events = await _eventService.GetAllEventsAsync();
            return Ok(events);
        }

        // GET: api/events/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetEventById(int id)
        {
            var ev = await _eventService.GetEventByIdAsync(id);
            if (ev == null)
                return NotFound(new { message = $"Event with ID {id} not found." });

            return Ok(ev);
        }

        //// GET: api/events/{id}/seats
        //[HttpGet("{id:int}/seats")]
        //public async Task<IActionResult> GetSeatsByEventId(int id)
        //{
        //    var seats = await _eventService.GetSeatsByEventIdAsync(id);
        //    return Ok(seats);
        //}

        // POST: api/events
        [HttpPost("create")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateEvent([FromForm] EventDto dto)
        {
            ModelState.Remove("ImageUrl");

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new
                    {
                        Field = x.Key,
                        Errors = x.Value.Errors.Select(e => e.ErrorMessage)
                    });

                return BadRequest(errors);
            }


            var role = User.FindFirstValue(ClaimTypes.Role);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = User.FindFirstValue(ClaimTypes.Name);
            if (role == "Admin")
            {
                dto.ManagerId = null;
                dto.ManagerName = "Admin";
            }
            else if (role == "Manager")
            {
                dto.ManagerId = string.IsNullOrWhiteSpace(userId) ? null : int.Parse(userId);
                dto.ManagerName = userName ?? "Unknown";
            }
            else
            {
                return BadRequest(new { message = "Unauthorized role" });
            }



            // Handle Image Upload
            if (dto.ImageFile != null)
            {
                var allowedExtensions = new[] { ".jpeg", ".jpg", ".png", ".webp" };
                var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "Uploads");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var ext = Path.GetExtension(dto.ImageFile.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(ext))
                    return BadRequest(new { message = "Invalid file type." });

                var uniqueFile = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploadsFolder, uniqueFile);

                using var fileStream = new FileStream(filePath, FileMode.Create);
                await dto.ImageFile.CopyToAsync(fileStream);

;
                dto.ImageUrl = $"/Uploads/{uniqueFile}";

            }
            else
            {

                dto.ImageUrl = $"/Uploads/no-image.png";

            }

            await _eventService.AddEventAsync(dto);

            return Ok(new { message = "Event created successfully" });
        }

        // PUT: api/events/{id}
        [HttpPut("update/{id:int}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateEvent(int id, [FromForm] EventDto dto)
        {
            ModelState.Remove("ImageUrl");

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(ms => ms.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                return BadRequest(new { message = "Validation Failed", errors });
            }

            var existing = await _eventService.GetEventByIdAsync(id);
            if (existing == null)
                return NotFound(new { message = $"Event with ID {id} not found." });

            if (dto.ImageFile != null)
            {
                var allowedExtensions = new[] { ".jpeg", ".jpg", ".png", ".webp" };
                var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "Uploads");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var ext = Path.GetExtension(dto.ImageFile.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(ext))
                    return BadRequest(new { message = "Invalid file type." });

                var uniqueFile = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploadsFolder, uniqueFile);

                using var fileStream = new FileStream(filePath, FileMode.Create);
                await dto.ImageFile.CopyToAsync(fileStream);

                
                dto.ImageUrl = $"/Uploads/{uniqueFile}";

            }
            else
            {
                dto.ImageUrl = existing.ImageUrl;

            }

            var isAdmin = User.IsInRole("Admin");
            // ---- Update event ----
            await _eventService.UpdateEventAsync(id, dto, isAdmin);

            return Ok(new { message = "Event updated successfully", dto });
        }

        // DELETE: api/events/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var existing = await _eventService.GetEventByIdAsync(id);
            if (existing == null)
                return NotFound(new { message = $"Event with ID {id} not found." });

            await _eventService.DeleteEventAsync(id);
            return NoContent();
        }

        [HttpGet("approved")]
        public async Task<IActionResult> GetApprovedEvents()
        {
            var events = await _eventService.GetApprovedEventsAsync();
            return Ok(events);
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPagedEvents([FromQuery] PagedRequest req)
        {
            var result = await _eventService.GetPagedEventAsync(req);
            return Ok(result);
        }


        [HttpGet("promoted")]
        public async Task<IActionResult> GetPromotedEvents()
        {
            var events = await _eventService.GetPromotedEventsAsync();
            return Ok(events);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id:int}/toggle-promotion")]
        public async Task<IActionResult> TogglePromotion(int id)
        {
            if (!await _eventService.EventExistsAsync(id))
                return NotFound();

            await _eventService.TogglePromotionAsync(id);

            return Ok();
        }


    }
}
