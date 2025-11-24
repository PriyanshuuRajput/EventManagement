using System.ComponentModel.DataAnnotations;

namespace Applications.Dto.OrganizerDto
{
    public class ManagerProfileDto
    {

        // public int UserId { get; set; }

        [Required(ErrorMessage = "Old password is required")]
        public string OldPassword { get; set; } = "";

        [Required(ErrorMessage = "New password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string NewPassword { get; set; } = "";

        [Required(ErrorMessage = "Confirm password is required")]
        [Compare("NewPassword", ErrorMessage = "Password do not match")]
        public string ConfirmPassword { get; set; } = "";

        [Required(ErrorMessage = "First name is required")]
        [MinLength(2, ErrorMessage = "First name must be at least 2 characters")]
        public string? FirstName { get; set; }
        //[Required(ErrorMessage = "Last name is required.")]
        //[MinLength(2, ErrorMessage = "Last name must be at least 2 characters")]
        public string? LastName { get; set; }

        public string ManagerName { get; set; } = "";

        public string? Image { get; set; }

        [Required(ErrorMessage = "Mobile number is required")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Mobile number must be 10 digits")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Only digits allowed")]
        public string Mobile { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; } = string.Empty;
        public string OrganizationName { get; set; } = string.Empty;

        [RegularExpression(@"^[A-Z]{5}[0-9]{4}[A-Z]{1}$", ErrorMessage = "Invalid PAN format")]
        public string PAN { get; set; } = string.Empty;

        //public string Email { get; set; }

        //[EmailAddress]
        public string AlternateEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is required.")]
        public int CityId { get; set; }
        [Required(ErrorMessage = "State is required")]
        public Guid? StateId { get; set; }
        [Required(ErrorMessage = "Country is required")]
        public Guid? CountryId { get; set; }

        // public DateTime? UpdatedAt { get; set; }
        public bool AcceptTerms { get; set; }
    }
}
