using System.ComponentModel.DataAnnotations;

namespace Domains.Entities
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        public ICollection<AdminUser>? Users { get; set; }
    }
}
