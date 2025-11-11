using System.ComponentModel.DataAnnotations;

namespace Applications.Dto.AccountDto
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Username/Email/Mobile is required ")]
        public string Identifier { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;

    }
}
