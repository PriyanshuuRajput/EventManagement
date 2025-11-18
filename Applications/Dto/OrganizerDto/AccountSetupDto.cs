using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;

namespace Applications.Dto.OrganizerDto
{
    public class AccountSetupDto
    {
        // ===============================
        // Details
        // ===============================

        [Required(ErrorMessage = "Organization Name is required")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "PAN number is required")]
        [RegularExpression(@"^[A-Z]{5}[0-9]{4}[A-Z]{1}$", ErrorMessage = "Invalid PAN format")]
        public string PAN { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required")]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string Address { get; set; } = string.Empty;

        // ===============================
        //  Person
        // ===============================

        [Required(ErrorMessage = "Contact person name is required")]
        public string ContactName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mobile number is required")]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Invalid mobile number format")]
        public string Mobile { get; set; } = string.Empty;

        // ===============================
        // Bank Details
        // ===============================

        [Required(ErrorMessage = "Bank beneficiary name is required")]
        [StringLength(100, ErrorMessage = "Beneficiary name cannot exceed 100 characters")]
        public string BankBeneficiary { get; set; } = string.Empty;

        [Required(ErrorMessage = "Account type is required")]
        [RegularExpression(@"^(Saving|Current|Other)$", ErrorMessage = "Account type must be Saving, Current, or Other")]
        public string AccountType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Bank name is required")]
        [StringLength(100, ErrorMessage = "Bank name cannot exceed 100 characters")]
        public string BankName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Account number is required")]
        [RegularExpression(@"^\d{9,18}$", ErrorMessage = "Invalid account number (must be 9–18 digits)")]
        public string AccountNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "IFSC code is required")]
        [RegularExpression(@"^[A-Z]{4}0[A-Z0-9]{6}$", ErrorMessage = "Invalid IFSC code format")]
        public string IFSC { get; set; } = string.Empty;

        // ===============================
        // Documents
        // ===============================

        [Required(ErrorMessage = "PAN document is required")]
        public IBrowserFile? PAnDocument { get; set; }

        [Required(ErrorMessage = "GSTIN document is required")]
        public IBrowserFile? GSTINDocument { get; set; }

        [Required(ErrorMessage = "Address proof is required")]
        public IBrowserFile? AddressProof { get; set; }

        [Required(ErrorMessage = "Bank proof is required")]
        public IBrowserFile? BankProof { get; set; }

        // ===============================
        // Agreement
        // ===============================

        [Range(typeof(bool), "true", "true", ErrorMessage = "You must accept the agreement before proceeding")]
        public bool AgreementAccepted { get; set; } = false;
    }
}
