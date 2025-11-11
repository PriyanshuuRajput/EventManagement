using Applications.Dto;
using Applications.Interfaces.IService;
using Microsoft.AspNetCore.Mvc;

namespace EventBooking_TicketManagement_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VenuesController : ControllerBase
    {
        private readonly IVenueService _venueService;

        public VenuesController(IVenueService venueService)
        {
            _venueService = venueService;
        }

        // GET: api/venues
        [HttpGet]
        public async Task<IActionResult> GetAllVenues()
        {
            var venues = await _venueService.GetAllVenuesAsync();
            return Ok(venues);
        }

        // GET: api/venues/{id}
        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetVenueById(int id)
        {
            var venue = await _venueService.GetVenueByIdAsync(id);
            if (venue == null)
                return NotFound(new { message = $"Venue with Id {id} not found." });

            return Ok(venue);
        }

        // POST: api/venues
        [HttpPost]
        public async Task<IActionResult> CreateVenue([FromBody] VenueDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _venueService.AddVenueAsync(dto);
                return Ok(new { message = "Venue created successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/venues/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateVenue(int id, [FromBody] VenueDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _venueService.UpdateVenueAsync(id, dto);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/venues/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteVenue(int id)
        {
            await _venueService.DeleteVenueAsync(id);
            return NoContent();
        }
    }
}
