using Applications.Dto;
using Applications.Interfaces.IRepository;
using Applications.Interfaces.IService;
using Microsoft.AspNetCore.Mvc;

namespace EventBooking_TicketManagement_API.Controllers
{
    [Route("api/countries")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly ICountryService _countryService;

        public CountryController(ICountryService countryService)
        {
            _countryService = countryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CountryDto>>> GetAllCountries()
        {
            var countries = await _countryService.GetAllAsync();
            return Ok(countries);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetCountryById(Guid id)
        {
            var country = await _countryService.GetByIdAsync(id);
            if (country == null)
                return NotFound(new { message = "Country not found" });
            return Ok(country);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCountry([FromBody] CountryDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdCountry = await _countryService.CreateAsync(request);
            return CreatedAtAction(nameof(GetCountryById), new { id = createdCountry.Id }, createdCountry);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateCountry(Guid id, [FromBody] CountryDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedCountry = await _countryService.UpdateAsync(id, request);
            if (updatedCountry == null)
                return NotFound(new { message = "Country not found" });

            return Ok(updatedCountry);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteCountry(Guid id)
        {
            var deleted = await _countryService.DeleteAsync(id);
            if (!deleted)
                return NotFound(new { message = "Country not found" });

            return NoContent();
        }
    }

}

