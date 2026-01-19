using Applications.Dto.UserDto;
using Applications.Interfaces.IService;
using EventBooking_TicketManagement_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EventBooking_TicketManagement_API.Controllers
{
    [ApiController]
    [Route("api/user")]
    [Authorize(Roles = "User")]
    public class RealUserController : ControllerBase
    {
        private readonly IRealUserService _realUserService;
        public RealUserController( IRealUserService realUserService)
        {
            _realUserService = realUserService;
            
        }

        [HttpGet("profile")]
        public async Task< IActionResult> GetProfile()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var profile = await _realUserService.GetUserAsync(userId);
            if (profile == null)
            {
                return NotFound("USer not found");

            }
            return Ok(profile);
        }
        [HttpPut("profile-update")]
        public async Task<IActionResult> UpdateProfile([FromBody] RealUserDto dto)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await _realUserService.UpdateUserAsync(userId, dto);
            if (result != "Success")
                return BadRequest(new { message = result });


            return Ok(result);
        }

        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage( IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var imagePath = await _realUserService.UploadProfileImageAsync(userId, file);

            return Ok(new ImageDto { Image = imagePath });
        }

    }
}
