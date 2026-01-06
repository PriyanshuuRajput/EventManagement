using Applications.Dto;
using Applications.Interfaces.IService;
using Microsoft.AspNetCore.Mvc;

namespace EventBooking_TicketManagement_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeBannerController : ControllerBase
    {
        private readonly IHomeBannerService _homeBannerService;
        public HomeBannerController( IHomeBannerService homeBannerService)
        {
            _homeBannerService = homeBannerService;
        }

        [HttpGet]

        public async Task<IActionResult> GetALL()
        {
            var banners = await _homeBannerService.GetAllAsync();
            return Ok(banners);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var banners = await _homeBannerService.GetActiveAsync();
            return Ok(banners);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var banner = await _homeBannerService.GetByIdAsync(id);
            if (banner == null) return NotFound();

            return Ok(banner);
        }

        [HttpPost]
        public async Task<IActionResult> Create(HomeBannerDto dto)
        {
            await _homeBannerService.AddAsync(dto);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update(HomeBannerDto dto)
        {
            await _homeBannerService.UpdateAsync(dto);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _homeBannerService.DeleteAsync(id);
            return Ok();
        }
        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            var imagePath = await _homeBannerService.UploadImageAsync(file);
            return Ok(imagePath);
        }

    }
}
