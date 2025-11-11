using System.ComponentModel.DataAnnotations;

namespace Applications.Dto.AccountDto
{
    public class SignUpDto
    {

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone]
        public string? PhoneNumber { get; set; } = string.Empty;

        // Optional: let users choose role (User / Organizer)
        public string Role { get; set; } = "User";
    }
}