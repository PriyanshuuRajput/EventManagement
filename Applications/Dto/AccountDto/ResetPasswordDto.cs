using System.ComponentModel.DataAnnotations;

namespace Applications.Dto.AccountDto
{
    public class ResetPasswordDto
    {
        [Required(ErrorMessage = "Reset token is required")]
        public string? Token { get; set; }

        [Required(ErrorMessage = "New password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public string? NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm password is required")]
        [Compare(nameof(NewPassword),
            ErrorMessage = "New password and confirm password do not match")]
        public string? ConfirmPassword { get; set; }

        // Optional – returned from API after reset
        public string Role { get; set; } = string.Empty;
    }
}
