using System.ComponentModel.DataAnnotations;

namespace Applications.Dto.OrganizerDto
{
    public class ManagerSignUpDto
    {
        //public string ManagerName { get; set; } = string.Empty;
        [Required]
        [RegularExpression(
      @"^[A-Za-z0-9._%+-]+@gmail\.com$",
      ErrorMessage = "Enter a valid Gmail address.")]
        public string Email { get; set; } = string.Empty;


        //[RegularExpression(
        //    @"^(\+91)?[6-9]\d{9}$",
        //    ErrorMessage = "Enter a valid  phone number.")]
        //public string PhoneNumber { get; set; } = string.Empty;
        //public string Address { get; set; } = string.Empty;
    }
}
