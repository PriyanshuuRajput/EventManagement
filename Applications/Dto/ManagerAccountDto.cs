using System.ComponentModel.DataAnnotations;

public class ManagerAccountDto
{
    [Required(ErrorMessage = "Name is required")]
    [MinLength(2, ErrorMessage = "Name must be at least 2 characters")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Mobile number is required")]
    [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Mobile must be exactly 10 digits")]
    public string Mobile { get; set; } = "";

    [Required(ErrorMessage = "Address is required")]
    public string Address { get; set; } = "";

    public string Email { get; set; } = "";

    public string? Image { get; set; }

   
    public string? OldPassword { get; set; }

    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    public string? NewPassword { get; set; }

    [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match")]
    public string? ConfirmPassword { get; set; }
}
