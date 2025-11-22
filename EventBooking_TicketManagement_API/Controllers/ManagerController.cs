using Applications.Dto;
using Applications.Dto.OrganizerDto;
using Applications.Interfaces;
using Applications.Interfaces.IService;
using Domains.Entities;
using Infrastructures.DbContexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;



namespace EventBooking_TicketManagement_API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ManagerEventController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly IManagerService _managerService;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly AppDbContext _db;
        private readonly IPasswordHasher _passwordHasher;



        public ManagerEventController(IEventService eventService, IWebHostEnvironment hostEnvironment, AppDbContext db, IPasswordHasher password, IManagerService managerService)
        {
            _eventService = eventService;
            _hostEnvironment = hostEnvironment;
            _db = db;
            _passwordHasher = password;
            _managerService = managerService;
        }
        private async Task<Manager?> GetLoggedManager()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out var userId)) return null;

            return await _db.Managers
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.UserId == userId);
        }

        [Authorize]
        [HttpPost("manager-changepassword-profilesetup")]

        public async Task<IActionResult> MangerProfileUpdate([FromBody] ManagerProfileDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var result = await _managerService.ManagerProfileChangePassword(userId, dto);

            if (result != "Success")
                return BadRequest(new { message = result });

            return Ok(new { message = "Profile Completed successfully!" });
        }

        [HttpPost("create")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateEvent([FromForm] ManagerEventDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var manager = await GetLoggedManager();
            if (manager == null)
                return Unauthorized("Manager not found.");

            if (!manager.IsApproved)
                return Unauthorized("Manager is not approved by admin.");

            if (manager.User.ChangePassword)
                return Unauthorized("Please update your password before creating event.");

            //// Get AdminUser.Id from JWT
            //var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //if (!int.TryParse(userIdClaim, out var userId))
            //    return BadRequest("Invalid User Id in token.");

            //// Convert AdminUser.Id -> Manager.Id 
            //var manager = await _db.Managers.FirstOrDefaultAsync(m => m.UserId == userId);
            //if (manager == null)
            //    return BadRequest("Manager profile not found for this user. Please register as manager.");

            //int managerId = manager.Id;
            //string managerName = manager.ManagerName ?? string.Empty;

            // Handle Image Upload
            if (dto.ImageFile != null)
            {
                var allowed = new[] { ".jpeg", ".jpg", ".png", ".webp" };
                var ext = Path.GetExtension(dto.ImageFile.FileName).ToLowerInvariant();

                if (!allowed.Contains(ext))
                    return BadRequest("Invalid image file.");

                var folder = Path.Combine(_hostEnvironment.WebRootPath, "Uploads");
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                var file = $"{Guid.NewGuid()}{ext}";
                var path = Path.Combine(folder, file);

                using var stream = new FileStream(path, FileMode.Create);
                await dto.ImageFile.CopyToAsync(stream);

                dto.ImageUrl = $"{Request.Scheme}://{Request.Host}/Uploads/{file}";
            }

            var venue = await _db.Venues.FindAsync(dto.VenueId);
            dto.TotalTickets = venue?.Capacity ?? 0;

            var createdEvent = await _eventService.CreateEventAsync(dto, manager.Id, manager.ManagerName);

            return Ok(new
            {
                Message = "Event submitted successfully! Pending admin approval.",
                EventId = createdEvent.Id,
                Status = createdEvent.Status.ToString(),
                ManagerName = createdEvent.ManagerName
            });
        }



        //  Get all events created by this manager
        [HttpGet("my-events")]
        public async Task<IActionResult> GetMyEvents()
        {
            var manager = await GetLoggedManager();
            if (manager == null)
                return Unauthorized("Manager profile not found.");

            var events = await _eventService.GetManagerEventsAsync(manager.Id);
            return Ok(events);
        }




        [HttpPut("update/{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateMyEvent(int id, [FromForm] ManagerEventUpdateDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var manager = await _db.Managers.FirstOrDefaultAsync(m => m.UserId == userId);
            if (manager == null)
                return Unauthorized("Manager profile not found.");

            var existingEvent = await _eventService.GetEventByIdAsync(id);
            if (existingEvent == null)
                return NotFound("Event not found.");

            if (existingEvent.ManagerId != manager.Id)
                return Unauthorized("You can only edit your own events.");

            if (existingEvent.Status == EventStatus.PaymentConfirmed ||
                existingEvent.Status == EventStatus.AdminApproved)
                return BadRequest("This event is approved already. Editing needs admin approval.");

            // IMAGE UPLOAD
            if (dto.ImageFile != null)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var ext = Path.GetExtension(dto.ImageFile.FileName).ToLower();

                if (!allowedExtensions.Contains(ext))
                    return BadRequest("Invalid image file.");

                var folder = Path.Combine(_hostEnvironment.WebRootPath, "Uploads");
                Directory.CreateDirectory(folder);

                var fileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(folder, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await dto.ImageFile.CopyToAsync(stream);

                dto.ImageUrl = $"{Request.Scheme}://{Request.Host}/Uploads/{fileName}";
            }
            else
            {
                dto.ImageUrl = existingEvent.ImageUrl; // keep old image
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
                CityId = dto.CityId,
                TicketPrice = dto.TicketPrice,
                ImageUrl = dto.ImageUrl,

                ManagerId = existingEvent.ManagerId,
                ManagerName = existingEvent.ManagerName,
                Status = existingEvent.Status,
                CreatedAt = existingEvent.CreatedAt,
                ApprovedAt = existingEvent.ApprovedAt,
                TotalTickets = existingEvent.TotalTickets,
                SoldTickets = existingEvent.SoldTickets,
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
