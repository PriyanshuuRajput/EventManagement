using Applications.Dto;
using Infrastructures.DbContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventBooking_TicketManagement_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StateController : ControllerBase
    {
        private readonly AppDbContext _db;
        public StateController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("by-country/{countryId:guid}")]
        public async Task<ActionResult<IEnumerable<StateDto>>> GetStatesByCountry(Guid countryId)
        {
            var states = await _db.States
                .Where(s => s.CountryId == countryId)
                .Select(s => new StateDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    CountryId = s.CountryId,
                    CountryName = s.Country.Name
                })
                .OrderBy(s => s.Name)
                .ToListAsync();

            return Ok(states);
        }
    }

}
