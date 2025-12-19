//using System.ComponentModel.DataAnnotations;

//namespace Applications.Dto
//{
//    public class SeatDto
//    {
//        public int Id { get; set; }

//        [Required(ErrorMessage = "Seat number is required.")]
//        [StringLength(10, ErrorMessage = "Seat number cannot exceed 10 characters.")]
//        public string SeatNumber { get; set; } = string.Empty;

//        [Required(ErrorMessage = "Category is required.")]
//        [StringLength(50, ErrorMessage = "Category cannot exceed 50 characters.")]
//        public string Category { get; set; } = string.Empty;

//        [Range(0, double.MaxValue, ErrorMessage = "Price must be a non-negative value.")]
//        public decimal Price { get; set; }

//        public bool IsBooked { get; set; }

//        [Required(ErrorMessage = "EventId is required.")]
//        [Range(1, int.MaxValue, ErrorMessage = "EventId must be a positive number.")]
//        public int EventId { get; set; }

//        public int? BookingId { get; set; }

//    }
//}
