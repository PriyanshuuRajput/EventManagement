using Applications.Dto;
using Applications.Interfaces.IService;
using Microsoft.AspNetCore.Mvc;

namespace EventBooking_TicketManagement_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        // ✅ Create booking
        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] BookingRequest request)
        {
            if (request == null)
                return BadRequest("Invalid booking data.");

            var result = await _bookingService.CreateBookingAsync(request);

            return Ok(new
            {
                Message = "Booking created successfully!",
                Booking = result
            });
        }

        // ✅ Get all bookings
        [HttpGet]
        public async Task<IActionResult> GetAllBookings()
        {
            var bookings = await _bookingService.GetAllBookingAsync();
            return Ok(bookings);
        }

        // ✅ Get booking by user
        [HttpGet("user/{email}")]
        public async Task<IActionResult> GetByUser(string email)
        {
            var bookings = await _bookingService.GetBookingByUserAsync(email);
            return Ok(bookings);
        }

        // Cancel booking and release seats
        [HttpPost("{bookingId}/cancel")]
        public async Task<IActionResult> CancelBooking(int bookingId)
        {
            try
            {
                await _bookingService.CancelBookingAsync(bookingId);
                return Ok(new { message = "Booking cancelled and seats released successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
