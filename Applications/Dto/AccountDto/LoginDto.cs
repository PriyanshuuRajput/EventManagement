using System.ComponentModel.DataAnnotations;

namespace Applications.Dto.AccountDto
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Username or Email is required ")]
        [RegularExpression(
            @"^(?:[A-Za-z0-9._%+-]+@gmail\.com|[A-Za-z0-9._-]{4,20})$",
            ErrorMessage = "Enter a valid Gmail address or a username (4–20 characters).")]
        public string Identifier { get; set; } = string.Empty;
        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public string Password { get; set; } = string.Empty;

    }
}
