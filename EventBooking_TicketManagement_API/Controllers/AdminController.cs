//using Applications.Dto;
//using Applications.Interfaces.IService;
//using Infrastructures.DbContexts;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace EventBooking_TicketManagement_API.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class AdminController : ControllerBase
//    {
//        private readonly AppDbContext _db;
//        private readonly IEventService eventService;
//        private readonly ISeatService seatService;
//        private readonly IVenueService venueService;
//        private readonly ICityService cityService;
//        private readonly IBookingService bookingService;

//        public AdminController(
//            AppDbContext db,
//            IEventService eventService,
//            ISeatService seatService,
//            IVenueService venueService,
//            ICityService cityService,
//            IBookingService bookingService)
//        {
//            _db = db;
//            this.eventService = eventService;
//            this.seatService = seatService;
//            this.venueService = venueService;
//            this.cityService = cityService;
//            this.bookingService = bookingService;
//        }

//        // ---------------- Events ----------------
//        [HttpGet("events")]
//        public async Task<IActionResult> GetAllEvents() =>
//            Ok(await eventService.GetAllEventsAsync());

//        [HttpPost("events")]
//        public async Task<IActionResult> CreateEvent([FromBody] EventDto dto)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(ModelState);

//            await eventService.AddEventAsync(dto);
//            return Ok(new { message = "Event created successfully" });
//        }

//        [HttpPut("events/{id}")]
//        public async Task<IActionResult> UpdateEvent(int id, [FromBody] EventDto dto)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(ModelState);

//            await eventService.UpdateEventAsync(id, dto);
//            return Ok(new { message = "Event updated successfully" });
//        }

//        [HttpDelete("events/{id}")]
//        public async Task<IActionResult> DeleteEvent(int id)
//        {
//            await eventService.DeleteEventAsync(id);
//            return Ok(new { message = "Event deleted successfully" });
//        }

//        // ---------------- Venues ----------------
//        [HttpGet("venues")]
//        public async Task<IActionResult> GetAllVenues() =>
//            Ok(await venueService.GetAllVenuesAsync());

//        [HttpPost("venues")]
//        public async Task<IActionResult> CreateVenue([FromBody] VenueDto dto)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(ModelState);

//            await venueService.AddVenueAsync(dto);
//            return Ok(new { message = "Venue created successfully" });
//        }

//        [HttpPut("venues/{id}")]
//        public async Task<IActionResult> UpdateVenue(Guid id, [FromBody] VenueDto dto)
//        {
//            await venueService.UpdateVenueAsync(id, dto);
//            return Ok(new { message = "Venue updated successfully" });
//        }

//        [HttpDelete("venues/{id}")]
//        public async Task<IActionResult> DeleteVenue(Guid id)
//        {
//            await venueService.DeleteVenueAsync(id);
//            return Ok(new { message = "Venue deleted successfully" });
//        }

//        // ---------------- Seats ----------------
//        [HttpGet("seats")]
//        public async Task<IActionResult> GetAllSeats() =>
//            Ok(await seatService.GetAllSeatsAsync());

//        [HttpPost("seats")]
//        public async Task<IActionResult> CreateSeat([FromBody] SeatDto dto)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(ModelState);

//            await seatService.AddSeatAsync(dto);
//            return Ok(new { message = "Seat created successfully" });
//        }

//        [HttpPut("seats/{id}")]
//        public async Task<IActionResult> UpdateSeat(int id, [FromBody] SeatDto dto)
//        {
//            await seatService.UpdateSeatAsync(id, dto);
//            return Ok(new { message = "Seat updated successfully" });
//        }

//        [HttpDelete("seats/{id}")]
//        public async Task<IActionResult> DeleteSeat(int id)
//        {
//            await seatService.DeleteSeatAsync(id);
//            return Ok(new { message = "Seat deleted successfully" });
//        }

//        // ---------------- Cities ----------------
//        [HttpGet("cities")]
//        public async Task<IActionResult> GetAllCities() =>
//            Ok(await cityService.GetAllCitiesAsync());

//        [HttpPost("cities")]
//        public async Task<IActionResult> CreateCity([FromBody] CityDto dto)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(ModelState);

//            await cityService.AddCityAsync(dto);
//            return Ok(new { message = "City added successfully" });
//        }

//        [HttpPut("cities/{id}")]
//        public async Task<IActionResult> UpdateCity(int id, [FromBody] CityDto dto)
//        {
//            await cityService.UpdateCityAsync(id, dto);
//            return Ok(new { message = "City updated successfully" });
//        }

//        [HttpDelete("cities/{id}")]
//        public async Task<IActionResult> DeleteCity(int id)
//        {
//            await cityService.DeleteCityAsync(id);
//            return Ok(new { message = "City deleted successfully" });
//        }

//        // ---------------- Countries ----------------
//        [HttpGet("countries")]
//        public async Task<IActionResult> GetAllCountries()
//        {
//            var countries = await _db.Countries
//                .Select(c => new { c.Id, c.Name, c.IsoCode })
//                .ToListAsync();

//            return Ok(countries);
//        }

//        [HttpGet("countries/{id}")]
//        public async Task<IActionResult> GetCountryById(int id)
//        {
//            var country = await _db.Countries.FindAsync(id);
//            if (country == null)
//                return NotFound();

//            return Ok(country);
//        }

//        // ---------------- Bookings ----------------
//        [HttpGet("bookings")]
//        public async Task<IActionResult> GetAllBookings() =>
//            Ok(await bookingService.GetAllBookingsAsync());

//        [HttpDelete("bookings/{id}")]
//        public async Task<IActionResult> DeleteBooking(int id)
//        {
//            await bookingService.DeleteBookingAsync(id);
//            return Ok(new { message = "Booking deleted successfully" });
//        }
//    }
//}
