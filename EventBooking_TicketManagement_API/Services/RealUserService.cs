using Applications.Dto.UserDto;
using Applications.Interfaces;
using Applications.Interfaces.IRepository;
using Applications.Interfaces.IService;
using Domains.Entities;
using Infrastructures.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace EventBooking_TicketManagement_API.Services
{
    public class RealUserService : IRealUserService
    {
        private readonly IRealUserRepository _realUserRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public RealUserService(IRealUserRepository realUserRepository,IPasswordHasher passwordHasher, AppDbContext context, IWebHostEnvironment env)
        {
            _realUserRepository = realUserRepository;
            _passwordHasher = passwordHasher;
            _context = context;
            _env = env;
        }
        public async Task<RealUserDto?> GetUserAsync(int userId)
        {
            var user = await _realUserRepository.GetRealUserAsync(userId);

            if (user == null)
            {
                // create empty profile automatically
                user = new RealUser
                {
                    UserId = userId,
                    FirstName = "",
                    LastName = "",
                    Address = ""
                };

                await _realUserRepository.CreateAsync(user);
                await _realUserRepository.SaveChangesAsync();
            }

            return new RealUserDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.User!.Email,
                Mobile = user.User.PhoneNumber ?? "",
                Address = user.Address ?? "",
                Image = user.Image
            };
        }


        public async Task<string> UpdateUserAsync(int userId, RealUserDto dto)
        {
            var user = await _realUserRepository.GetRealUserAsync(userId);
            if (user == null) return "User not Found";
            var u = user.User;
            if (u == null) return "Auth user not found";

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Address = dto.Address;
            user.Image = dto.Image;

            u.PhoneNumber = dto.Mobile;
            u.UpdatedAt = DateTime.UtcNow;

            if (!string.IsNullOrWhiteSpace(dto.OldPassword))
            {
                if (!_passwordHasher.VerifyPassword(dto.OldPassword, u.PasswordHash))
                    return "Old Password is incorrect.";
                if (dto.NewPassword != dto.ConfirmPassword) return "Passwords do not match";
                u.PasswordHash = _passwordHasher.HashPassword(dto.NewPassword);
                u.ChangePassword = false;
            }
            await _realUserRepository.SaveChangesAsync();
            return "Success";
        }
        public async Task<string> UploadProfileImageAsync(int userId, IFormFile file)
        {
            var user = await _context.RealUsers
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (user == null)
                throw new Exception("User not found");

            var folderPath = Path.Combine(_env.WebRootPath, "profile-images");
            Directory.CreateDirectory(folderPath);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(folderPath, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            user.Image = $"/profile-images/{fileName}";
            user.User!.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return user.Image!;
        }

    }
}
