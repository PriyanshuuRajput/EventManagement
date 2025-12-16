using Applications.Dto;
using Applications.Interfaces.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace EventBooking_TicketManagement_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventCategoryController : ControllerBase
    {
        private readonly IEventCategoryService _eventCategoryService;

        public EventCategoryController(IEventCategoryService eventCategoryService)
        {
            _eventCategoryService = eventCategoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _eventCategoryService.GetAllAsync();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var category = await _eventCategoryService.GetByIdAsync(id);
            if (category == null) return NotFound();
            return Ok(category);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] EventCategoryDto dto)
        {
            try
            {
                ModelState.Remove(nameof(dto.ImageUrl));

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (dto.ImageFile != null)
                {
                    dto.ImageUrl = await SaveImage(dto.ImageFile);
                }

                var result = await _eventCategoryService.CreateAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "An error occurred while creating the event category.",
                    error = ex.Message
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] EventCategoryDto dto)
        {
            var existing = await _eventCategoryService.GetByIdAsync(id);
            if (existing == null) return NotFound();

            ModelState.Remove(nameof(dto.ImageUrl));
            ModelState.Remove(nameof(dto.Slug));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.ImageFile != null)
            {
                dto.ImageUrl = await SaveImage(dto.ImageFile);
            }
            else
            {
                dto.ImageUrl = existing.ImageUrl;
            }

            await _eventCategoryService.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _eventCategoryService.DeleteAsync(id);
            return NoContent();
        }

        private async Task<string> SaveImage(IFormFile file)
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "category-images");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/category-images/{fileName}";
        }
    }
}