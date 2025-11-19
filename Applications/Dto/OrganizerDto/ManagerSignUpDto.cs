using System.ComponentModel.DataAnnotations;

namespace Applications.Dto.OrganizerDto
{
    public class ManagerSignUpDto
    {
        //public string ManagerName { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        //public string Address { get; set; } = string.Empty;
    }
}
