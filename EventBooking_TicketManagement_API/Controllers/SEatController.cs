//using Applications.Dto;
//using Applications.Interfaces.IService;
//using Microsoft.AspNetCore.Mvc;
//using System.ComponentModel.DataAnnotations;

//namespace EventBooking_TicketManagement_API.Controllers
//{
//    [Route("api/seats")]
//    [ApiController]
//    public class SEatController : ControllerBase
//    {
//        private readonly ISeatService _seatService;

//        public SEatController(ISeatService seatService)
//        {
//            _seatService = seatService;
//        }

//        //[HttpGet("event/{eventId}")]
//        //public async Task<IActionResult> GetSeatsByEvent(int eventId)
//        //{
//        //    var seats = await _seatService.GetSeatsByEventAsync(eventId);

//        //    if (seats == null || !seats.Any())
//        //        return Ok(new List<SeatDto>());

//        //    return Ok(seats);
//        //}

//        [HttpPost("event/{eventId}/add")]

//        public async Task<IActionResult> AddSeats(int eventId, [FromBody] AddSeatRequest request)
//        {
//            if (request == null || request.TotalSeats <= 0)
//                return BadRequest("Invalid seat request");

//            try
//            {
//                await _seatService.AddSeatsAsync(eventId, request.TotalSeats, request.Category.Trim(), request.Price);
//                return Ok($"Successfully added {request.TotalSeats} seats for event {eventId}.");
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(new { message = ex.Message });
//            }
//        }

//        [HttpGet("event/{eventId}/summary")]
//        public async Task<IActionResult> GetSeatSummary(int eventId)
//        {
//            var summary = await _seatService.GetSeatSummaryAsync(eventId);
//            return Ok(new
//            {
//                EventId = eventId,
//                Total = summary.total,
//                Booked = summary.booked,
//                Available = summary.available
//            });
//        }

//        [HttpGet]
//        public async Task<IActionResult> GetALlSeats()
//        {
//            var seats = await _seatService.GetAllSeatsAsync();

//            return Ok(seats);
//        }

//        public class AddSeatRequest
//        {
//            public int TotalSeats { get; set; }

//            [Required(ErrorMessage = "Category is required.")]
//            public string Category { get; set; } = string.Empty;

//            [Range(0, double.MaxValue, ErrorMessage = "Price must be non-negative.")]
//            public decimal Price { get; set; }
//        }
//    }
//}
