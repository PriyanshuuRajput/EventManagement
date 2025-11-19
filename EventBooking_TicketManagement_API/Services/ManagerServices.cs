using Applications.Dto.OrganizerDto;
using Applications.Interfaces.IRepository;
using Applications.Interfaces.IService;
using Domains.Entities;

namespace EventBooking_TicketManagement_API.Services
{
    public class ManagerServices : IManagerService
    {
        private readonly IManagerRepository _managerRepo;
        private readonly IEmailService _emailService;

        public ManagerServices(IManagerRepository managerRepo, IEmailService emailService)
        {
            _managerRepo = managerRepo;
            _emailService = emailService;
        }

        public async Task<string> SignUpManagerAsync(ManagerSignUpDto dto)
        {
            // Validation: At least 1 field must be filled
            if (string.IsNullOrWhiteSpace(dto.Email) && string.IsNullOrWhiteSpace(dto.PhoneNumber))
                return "Email or Phone number is required.";

            // Check if email exists (if provided)
            if (!string.IsNullOrWhiteSpace(dto.Email) &&
                await _managerRepo.EmailExists(dto.Email))
                return "Email already registered";

            // Generate username
            string username = dto.Email != null
                ? dto.Email.Split('@')[0] + new Random().Next(1000, 9999)
                : "mgr" + new Random().Next(100000, 999999);

            // Create AdminUser (no password yet)
            var user = new AdminUser
            {
                Email = dto.Email ?? "",
                Username = username,
                PhoneNumber = dto.PhoneNumber,
                RoleId = 3,               // Manager Role
                IsActive = true,
                ChangePassword = true,    // Manager must change password after approval
                PasswordHash = ""         // Admin will assign temporary password
            };

            user = await _managerRepo.CreateAdminUser(user);

            // Create Manager (minimal data)
            var manager = new Manager
            {
                UserId = user.Id,
                ManagerName = "",         // Manager will fill after approval
                Mobile = dto.PhoneNumber ?? "",
                Address = "",
                IsApproved = false,
                CreatedAt = DateTime.UtcNow
            };

            await _managerRepo.CreateManager(manager);

            return "Manager signup request submitted. Awaiting admin approval.";
        }


        public async Task<string> ApproveManagerAsync(int managerId)
        {
            var manager = await _managerRepo.GetManagerWithUserAsync(managerId);
            if (manager == null) return "Manager not found.";
            if (manager.IsApproved) return "Already approved";

            string tempPassword = GenerateTempPassword();

            manager.User.PasswordHash = BCrypt.Net.BCrypt.HashPassword(tempPassword);
            manager.IsApproved = true;
            manager.ApprovedAt = DateTime.Now;

            await _managerRepo.SaveChangesAsync();

            // Send Email
            await _emailService.SendEmailAsync(
                manager.User.Email,
                "Your Manager Account is Approved ✔",
                $@"
                <h3>Welcome to EventiGo</h3>
                <p>Your manager account is now approved.</p>
                <p><b>Temporary Password:</b> {tempPassword}</p>
                <p>Please log in and change your password immediately.</p>
                "
            );

            return "Manager approved and email sent.";
        }
        private string GenerateTempPassword()
        {
            return Guid.NewGuid().ToString("N")[..8];
        }

        public async Task<List<object>> GetPendingManagersAsync()
        {
            var pending = await _managerRepo.GetPendingManagersAsync();

            return pending.Select(m => new
            {
                m.Id,
                Email = m.User.Email,
                Phone = m.User.PhoneNumber,
                m.CreatedAt

            }).ToList<object>();
        }

        public async Task RejectManagerAsync(int managerId)
        {
            var manager = await _managerRepo.GetManagerByIdAsync(managerId);

            if (manager == null)
                throw new Exception("Manager not found.");

            _managerRepo.DeleteManager(manager);

            await _managerRepo.SaveChangesAsync();
        }
    }


}
