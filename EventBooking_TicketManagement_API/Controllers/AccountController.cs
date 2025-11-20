using Applications.Dto.AccountDto;
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
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IJwtTokenService _jwt;
        private readonly AppDbContext _db;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IEmailService _emailService;

        public AccountController(IJwtTokenService jwt, AppDbContext db, IPasswordHasher passwordHasher, IEmailService emailService)
        {
            _jwt = jwt;
            _db = db;
            _passwordHasher = passwordHasher;
            _emailService = emailService;
        }

        // LOGIN: username/email/phone
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Identifier))
                return BadRequest("Username, Email, or Phone Number is required.");

            var user = _db.Users
                .Include(u => u.Role)
                .Include(u => u.Manager)
                .FirstOrDefault(u =>
                u.Username == dto.Identifier ||
                u.Email == dto.Identifier ||
                u.PhoneNumber == dto.Identifier
        );



            if (user == null || !_passwordHasher.VerifyPassword(dto.Password, user.PasswordHash))
                return Unauthorized("Invalid credentials.");

            if (user.Role?.Name == "Manager")
            {
                if (user.Manager != null && !user.Manager.IsApproved)
                {
                    return Unauthorized("Your manager account is not approved by admin.");
                }

                if (user.ChangePassword)
                {
                    string token = _jwt.GenerateToken(user);
                    return Ok(new
                    {
                        ChangePassword = true,
                        token = token,
                        message = "Passsword change required.",
                        userId = user.Id
                    });
                }
            }



            // Generate JWT
            var loginToken = _jwt.GenerateToken(user);

            //// Store in HttpOnly cookie
            //Response.Cookies.Append("jwt", token, new CookieOptions
            //{
            //    HttpOnly = true,
            //    Secure = true,
            //    SameSite = SameSiteMode.Strict,
            //    Expires = DateTime.UtcNow.AddHours(1)
            //});

            return Ok(new
            {
                message = "Login successful",
                role = user.Role?.Name,
                token = loginToken,
                userId = user.Id
            });
        }

        // SIGNUP: User or Organizer
        [HttpPost("signup")]
        public IActionResult Signup([FromBody] SignUpDto dto)
        {
            if (_db.Users.Any(u => u.Email == dto.Email))
                return BadRequest("Email already registered");

            if (!string.IsNullOrEmpty(dto.PhoneNumber) &&
                _db.Users.Any(u => u.PhoneNumber == dto.PhoneNumber))
                return BadRequest("Phone number already registered");

            // Prevent admin signup
            if (dto.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Admin accounts cannot be created via signup.");

            // Force managers to use manager-signup API
            if (dto.Role.Equals("Manager", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Managers must sign up using /api/account/manager-signup");

            // Generate username
            string baseUsername = dto.Email.Split('@')[0];
            string username;
            var random = new Random();

            do
            {
                username = $"{baseUsername}{random.Next(1000, 9999)}";
            }
            while (_db.Users.Any(u => u.Username == username));

            // Normal users only
            int roleId = 3;

            var user = new AdminUser
            {
                Username = username,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                RoleId = roleId,
                PasswordHash = _passwordHasher.HashPassword(dto.Password),
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _db.Users.Add(user);
            _db.SaveChanges();

            return Ok(new
            {
                message = $"Signup successful! Your username is {username}.",
                username = username
            });
        }


        //// Dashboard 
        //[Authorize]
        //[HttpGet("dashboard")]
        //public IActionResult Dashboard()
        //{
        //    var username = User.Identity?.Name ?? "Unknown";
        //    return Ok(new { message = $"Welcome {username}!" });
        //}

        [HttpPost("manager-signup")]
        public async Task<IActionResult> ManagerSignup([FromBody] ManagerSignUpDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest("Email is required.");

            if (!string.IsNullOrWhiteSpace(dto.Email) &&
                _db.Users.Any(u => u.Email == dto.Email))
                return BadRequest("Email already registered");

            //if (!string.IsNullOrWhiteSpace(dto.PhoneNumber) &&
            //    _db.Users.Any(u => u.PhoneNumber == dto.PhoneNumber))
            //    return BadRequest("Phone number already registered");

            // Generate username
            string username = dto.Email != null
                ? dto.Email.Split('@')[0] + new Random().Next(1000, 9999)
                : "mgr" + new Random().Next(100000, 999999);

            // Manager RoleId = 2 (adjust based on your DB)
            var user = new AdminUser
            {
                Username = username,
                Email = dto.Email ?? "",
                PhoneNumber = "",
                RoleId = 2,
                PasswordHash = "",
                ChangePassword = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _db.Users.Add(user);
            _db.SaveChanges();

            // Create a manager request entry
            var manager = new Manager
            {
                UserId = user.Id,
                ManagerName = "",
                Address = "",
                Mobile = "",
                IsApproved = false,
                CreatedAt = DateTime.UtcNow
            };

            _db.Managers.Add(manager);
            _db.SaveChanges();


            string adminEmail = "rajputronak0058@gmail.com";

            await _emailService.SendEmailAsync(
                adminEmail,
                "New Manager Signup Request",
            $@"
            <h2>New Manager Request</h2>
            <p>A new manager has applied for approval.</p>
            <p><b>Email:</b> {dto.Email}</p>
            <p>Please review and approve them in the Admin Dashboard.</p>
            "
            );

            return Ok(new
            {
                message = "Manager signup request submitted. Admin will review and approve."
            });
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var user = await _db.Users.FindAsync(userId);
            if (user == null)
                return BadRequest("User not found.");

            if (!string.IsNullOrEmpty(user.PasswordHash) &&
                !_passwordHasher.VerifyPassword(dto.OldPassword, user.PasswordHash))
            {
                return BadRequest("Old password is incorrect.");
            }


            user.PasswordHash = _passwordHasher.HashPassword(dto.NewPassword);
            user.ChangePassword = false; // password update completed

            await _db.SaveChangesAsync();

            return Ok(new { message = "Password changed successfully." });
        }

        // Logout
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");
            return Ok(new { message = "Logged out successfully." });
        }
    }
}
