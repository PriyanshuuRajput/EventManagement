using System.ComponentModel.DataAnnotations;

namespace Domains.Entities
{
    public class AdminUser
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public Role? Role { get; set; }
        [Phone]
        public string? PhoneNumber { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        public bool ChangePassword { get; set; } = false;
        public Manager? Manager { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<Event>? Events { get; set; }
        public ICollection<ChangePasswordToken>? PasswordTokens { get; set; }
      
    }
}
