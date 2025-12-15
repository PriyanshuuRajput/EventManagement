using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Applications.Dto
{
    public class EventCategoryDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="Category Name is required.")]
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        [Required(ErrorMessage ="Image is required ")]
        public string? ImageUrl { get; set; }
    }
}
