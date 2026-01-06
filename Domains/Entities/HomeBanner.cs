using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domains.Entities
{
    public class HomeBanner
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Image { get; set; } = string.Empty;

        public string? Title { get; set; }

        [ForeignKey("EventId")]
        public Event? Event { get; set; }
        public int? EventId { get; set; }  

        public string? Link { get; set; }   

        public bool Status { get; set; } 

        public int Position { get; set; }   
    }
}
