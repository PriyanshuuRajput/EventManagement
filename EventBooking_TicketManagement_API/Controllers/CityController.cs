using Applications.Dto;
using Applications.Interfaces.IService;
using Microsoft.AspNetCore.Mvc;

namespace EventBooking_TicketManagement_API.Controllers
{
    [Route("api/cities")]
    [ApiController]
    public class CityController : ControllerBase
    {
        private readonly ICityService cityService;

        public CityController(ICityService cityService)
        {
            this.cityService = cityService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCities()
        {
            var cities = await cityService.GetAllCitiesAsync();
            return Ok(cities);
        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCityById(int id)
        {
            var city = await cityService.GetCityByIdAsync(id);
            if (city == null)
                return NotFound(new { message = $"City with Id {id} not found." });

            return Ok(city);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCity([FromBody] CityDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await cityService.AddCityAsync(dto);
            return Ok(new { message = "City added successfully." });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCity(int id, [FromBody] CityDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                await cityService.UpdateCityAsync(id, dto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCity(int id)
        {
            await cityService.DeleteCityAsync(id);
            return NoContent();
        }

    }
}
