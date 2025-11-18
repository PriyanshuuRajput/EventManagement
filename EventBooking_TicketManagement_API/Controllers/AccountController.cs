using Applications.Dto.AccountDto;
using Applications.Interfaces;
using Domains.Entities;
using Infrastructures.DbContexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventBooking_TicketManagement_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IJwtTokenService _jwt;
        private readonly AppDbContext _db;
        private readonly IPasswordHasher _passwordHasher;

        public AccountController(IJwtTokenService jwt, AppDbContext db, IPasswordHasher passwordHasher)
        {
            _jwt = jwt;
            _db = db;
            _passwordHasher = passwordHasher;
        }

        // LOGIN: username/email/phone
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Identifier))
                return BadRequest("Username, Email, or Phone Number is required.");

            var user = _db.Users
                .Include(u => u.Role)
                .FirstOrDefault(u =>
                u.Username == dto.Identifier ||
                u.Email == dto.Identifier ||
                u.PhoneNumber == dto.Identifier
        );



            if (user == null || !_passwordHasher.VerifyPassword(dto.Password, user.PasswordHash))
                return Unauthorized("Invalid credentials.");

            // Generate JWT
            var token = _jwt.GenerateToken(user);

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
                token = token,
                userId = user.Id
            });
        }

        // SIGNUP: User or Organizer
        [HttpPost("signup")]
        public IActionResult Signup([FromBody] SignUpDto dto)
        {
            if (_db.Users.Any(u => u.Email == dto.Email))
                return BadRequest("Email already registered");

            if (!string.IsNullOrEmpty(dto.PhoneNumber) && _db.Users.Any(u => u.PhoneNumber == dto.PhoneNumber))
                return BadRequest("Phone number already registered");

            // Prevent creating admin from signup
            if (dto.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                dto.Role = "User";

            // Generate username
            string baseUsername = dto.Email.Split('@')[0];
            string username;
            var random = new Random();

            do
            {
                username = $"{baseUsername}{random.Next(1000, 9999)}";
            }
            while (_db.Users.Any(u => u.Username == username));

            // Convert string role to roleId
            int roleId = dto.Role.ToLower() switch
            {
                "manager" => 2,
                "admin" => 1,   // only if allowed
                _ => 3          // default User
            };

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
                message = $"Signup successful! Your username is {username}. Redirecting to login...",
                username = username
            });
        }


        // Dashboard 
        [Authorize]
        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            var username = User.Identity?.Name ?? "Unknown";
            return Ok(new { message = $"Welcome {username}!" });
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
