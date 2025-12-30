using Applications.Dto;
using Applications.Interfaces.IService;
using EventBooking_TicketManagement_API.Helper;
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
        private readonly IBookingRepository _bookingRepository;
        private readonly IQrCodeService _qrCodeService;

        public BookingController(IBookingService bookingService , IBookingRepository bookingRepository, IQrCodeService qrCodeService)
        {
            _bookingService = bookingService;
            _bookingRepository = bookingRepository;
            _qrCodeService = qrCodeService;
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

        // ✅ Get all bookings 
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
            var userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            var bookings = await _bookingService.GetBookingByUserAsync(userId);
            return Ok(bookings);
        }
        [Authorize(Roles = "Manager")]
        [HttpGet("manager/earning")]
        public async Task<IActionResult> GetManagerBookingsAndEarning()
        {
            var managerIdClaim = User.FindFirst("ManagerId")?.Value;

            if (string.IsNullOrWhiteSpace(managerIdClaim))
                return Unauthorized("ManagerId missing in token");

            int managerId = int.Parse(managerIdClaim);

            var bookings = await _bookingService.GetBookingsByManagerAsync(managerId);

            return Ok(bookings);
        }

        [AllowAnonymous]
        [HttpGet("{bookingId}/ticket")]
        public async Task<IActionResult> DownloadTicket(int bookingId)
        {
            var booking = await _bookingRepository.GetBookingByIdAsync(bookingId)
                          ?? throw new Exception("Booking not found");

            var qrBytes = _qrCodeService.GenerateQr(booking.QrCode);
            var qrBase64 = $"data:image/png;base64,{Convert.ToBase64String(qrBytes)}";

            var html = TicketTemplate.TicketHtml(
                booking.Event!.Title,
                booking.Event.Venue!.VenueName,
                booking.Event.StartDate,
                booking.TicketCount,
                booking.TicketNumber,
                booking.Event.TicketPrice * booking.TicketCount,
                qrBase64
            );

            return Content(html, "text/html");
        }


        [HttpPost("scan")]
        public async Task<IActionResult> ScanTicket([FromQuery] string qr)
        {
            var booking = await _bookingRepository.GetBookingByQrAsync(qr);
            if (booking == null)
                return BadRequest("Invalid Ticket");

            if (booking.UsedEntries >= booking.TicketCount)
                return BadRequest("Entry limit reached");

            booking.UsedEntries++;
            await _bookingRepository.UpdateAsync(booking);

            return Ok("Entry Allowed");
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
