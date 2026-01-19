using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domains.Entities
{
    public class RealUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public AdminUser? User { get; set; }
     
        public string FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; }

        public string? Address { get; set; }
        public string? Image {  get; set; }

    }
}
