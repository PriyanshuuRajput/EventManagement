using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Applications.Dto.UserDto
{
    public class RealUserDto
    {

        public int Id { get; set; }
        [Required(ErrorMessage = "FirstName is required")]
        [MinLength(3, ErrorMessage = "FirstName must be at least 3 characters")]
        public string FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; }
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mobile number is required")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Mobile must be exactly 10 digits")]
        public string Mobile { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; } = string.Empty;

        public string? Image { get; set; }


        public string OldPassword { get; set; } = "";
        public string NewPassword { get; set; } = "";

        public string ConfirmPassword { get; set; } = "";

    }
}
