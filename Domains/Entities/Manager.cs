using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domains.Entities
{
    public class Manager
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public AdminUser? User { get; set; }

        public string ManagerName { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;
        //public string ContactName { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;

        //public string PAN { get; set; } = string.Empty;

        //public string BankBeneficiary { get; set; } = string.Empty;
        //public string AccountType { get; set; } = string.Empty;
        //public string BankName { get; set; } = string.Empty;
        //public string AccountNumber { get; set; } = string.Empty;
        //public string IFSC { get; set; } = string.Empty;

        //public string PANPath { get; set; } = string.Empty;
        //public string GSTINPath { get; set; } = string.Empty;
        //public string AddressProofPath { get; set; } = string.Empty;
        //public string BankProofPath { get; set; } = string.Empty;

        public bool IsApproved { get; set; } = false;
        public DateTime? ApprovedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<Event>? Events { get; set; }
        public bool IsProfileCompleted { get; set; }
    }
}
