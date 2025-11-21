using System.ComponentModel.DataAnnotations;

namespace Applications.Dto.OrganizerDto
{
    public class ManagerProfileDto
    {

        // public int UserId { get; set; }
        public string OldPassword { get; set; } = "";
        public string NewPassword { get; set; } = "";
        public string ConfirmPassword { get; set; } = "";

        public string ManagerName { get; set; } = "";

        public string? Image { get; set; }
        public string Mobile { get; set; } = "";
        public string Address { get; set; } = "";
        public string OrganizationName { get; set; } = string.Empty;

        [RegularExpression(@"^[A-Z]{5}[0-9]{4}[A-Z]{1}$", ErrorMessage = "Invalid PAN format")]
        public string PAN { get; set; } = string.Empty;

        public string Email { get; set; }

        [EmailAddress]
        public string AlternateEmail { get; set; } = "";

        public int CityId { get; set; }
        public Guid StateId { get; set; }

        // public DateTime? UpdatedAt { get; set; }
    }
}
