using Applications.Dto;
using Applications.Interfaces.IService;
using Microsoft.AspNetCore.Mvc;

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

        // GET: api/events/{id}/seats
        [HttpGet("{id:int}/seats")]
        public async Task<IActionResult> GetSeatsByEventId(int id)
        {
            var seats = await _eventService.GetSeatsByEventIdAsync(id);
            return Ok(seats);
        }

        // POST: api/events
        [HttpPost("create")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateEvent([FromForm] EventDto dto)
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


                dto.ImageUrl = $"/Uploads/{uniqueFile}";
            }
            else
            {
                // If no file uploaded and ImageUrl empty → assign a default
                if (string.IsNullOrWhiteSpace(dto.ImageUrl))
                    dto.ImageUrl = "/Uploads/no-image.png";
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
                // If no new image uploaded → keep old one
                dto.ImageUrl = string.IsNullOrWhiteSpace(existing.ImageUrl)
                    ? "/Uploads/no-image.png"
                    : existing.ImageUrl;
            }

            // ---- Update event ----
            await _eventService.UpdateEventAsync(id, dto);

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
    }
}
