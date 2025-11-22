using Applications.Dto.OrganizerDto;
using Applications.Interfaces;
using Applications.Interfaces.IRepository;
using Applications.Interfaces.IService;
using Domains.Entities;

namespace EventBooking_TicketManagement_API.Services
{
    public class ManagerServices : IManagerService
    {
        private readonly IManagerRepository _managerRepo;
        private readonly IEmailService _emailService;
        private readonly IPasswordHasher _passwordHasher;

        public ManagerServices(IManagerRepository managerRepo, IEmailService emailService, IPasswordHasher passwordHasher)
        {
            _managerRepo = managerRepo;
            _emailService = emailService;
            _passwordHasher = passwordHasher;
        }

        public async Task<string> SignUpManagerAsync(ManagerSignUpDto dto)
        {
            // Validation: At least 1 field must be filled
            if (string.IsNullOrWhiteSpace(dto.Email))
                return "Phone number is required.";

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
                PhoneNumber = "",
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
                Mobile = "",
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

            manager.User.PasswordHash = _passwordHasher.HashPassword(tempPassword);
            manager.IsApproved = true;
            manager.ApprovedAt = DateTime.Now;

            await _managerRepo.SaveChangesAsync();

            // Send Email

            string loginUrl = "https://localhost:7117/?login=true";

            string emailBody = $@"
<div style='font-family:Arial, sans-serif; background:#f5f7fa; padding:20px;'>
    <div style='max-width:600px; margin:auto; background:white; border-radius:10px; 
                box-shadow:0 4px 10px rgba(0,0,0,0.08); overflow:hidden;'>

        <!-- Header -->
        <div style='background:#4a6cf7; padding:20px; text-align:center; color:white;'>
            <h2 style='margin:0;'>EventiGo</h2>
            <p style='margin:0;'>Manager Account Approved ✔</p>
        </div>

        <!-- Body -->
        <div style='padding:25px;'>
            <p style='font-size:16px; color:#333;'>Hello,</p>

            <p style='font-size:15px; color:#555; line-height:1.6;'>
                Your manager account has been successfully approved.  
                Use the temporary password below to log in and complete your registration.
            </p>

            <div style='margin:20px 0; padding:15px; background:#f0f3ff; border-left:4px solid #4a6cf7;'>
                <p style='margin:0; font-size:16px; color:#333;'>
                    <b>Temporary Password:</b> {tempPassword}
                </p>
            </div>

            <p style='font-size:15px; color:#555;'>
                Please update your password immediately after logging in.
            </p>

            <!-- Button -->
            <div style='text-align:center; margin:30px 0;'>
                <a href='{loginUrl}' 
                   style='background:#4a6cf7; padding:12px 20px; color:white; 
                          text-decoration:none; border-radius:6px; font-size:16px;'>
                    Login to Manager Portal
                </a>
            </div>

            <p style='font-size:14px; color:#777; text-align:center;'>
                If you did not request this account, please ignore this email.
            </p>
        </div>

        <!-- Footer -->
        <div style='background:#f0f0f0; padding:10px; text-align:center; font-size:12px; color:#777;'>
            © EventiGo | Smart Event Booking & Management Platform
        </div>
    </div>
</div>
";

            await _emailService.SendEmailAsync(
        manager.User.Email,
        "Your Manager Account is Approved ✔",
        emailBody
    );


            return "Manager approved and email sent.";
        }
        private string GenerateTempPassword()
        {
            return Guid.NewGuid().ToString("N")[..8];
        }

        public async Task<string> ManagerProfileChangePassword(int userIdFromToken, ManagerProfileDto dto)
        {

            var manager = await _managerRepo.GetManagerWithUserByUserIdAsync(userIdFromToken);

            if (manager == null)
                return "Manager not Found";

            var user = manager.User;
            if (user == null) return "User account not found";

            if (!_passwordHasher.VerifyPassword(dto.OldPassword, user.PasswordHash))
                return "Temporary password is incorrect!";

            if (dto.NewPassword != dto.ConfirmPassword)
                return "New password and confirm password do not match.";

            if (!string.IsNullOrWhiteSpace(dto.NewPassword) || dto.NewPassword.Length > 6)
                return "New password must be at least 6 characters.";

            user.PasswordHash = _passwordHasher.HashPassword(dto.NewPassword);
            user.ChangePassword = false;


            manager.ManagerName = dto.ManagerName;
            manager.Mobile = dto.Mobile;
            manager.Address = dto.Address;
            manager.IsProfileCompleted = true;

            manager.UpdatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;

            await _managerRepo.SaveChangesAsync();

            return "Success";


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
