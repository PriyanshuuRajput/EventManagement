using Applications.Dto;
using Applications.Interfaces.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EventBooking_TicketManagement_API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        //  Create booking (User from JWT)
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] BookingRequest request)
        {
            if (request == null)
                return BadRequest("Invalid booking data.");

            int userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            var result = await _bookingService.CreateBookingAsync(request, userId);

            return Ok(new
            {
                Message = "Booking created successfully!",
                Booking = result
            });
        }

        // ✅ Get all bookings (Admin use)
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllBookings()
        {
            var bookings = await _bookingService.GetAllBookingAsync();
            return Ok(bookings);
        }

        // ✅ Get bookings of logged-in user
        [HttpGet("my")]
        public async Task<IActionResult> GetMyBookings()
        {
            int userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            var bookings = await _bookingService.GetBookingByUserAsync(userId);
            return Ok(bookings);
        }

        //  Cancel booking
        [HttpPost("{bookingId}/cancel")]
        public async Task<IActionResult> CancelBooking(int bookingId)
        {
            await _bookingService.CancelBookingAsync(bookingId);
            return Ok(new { message = "Booking cancelled successfully." });
        }
    }
}
