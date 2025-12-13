using Applications.Dto;
using Applications.Interfaces.IRepository;
using Applications.Interfaces.IService;
using Domains.Entities;

namespace EventBooking_TicketManagement_API.Services
{
    public class EventCategoryService : IEventCategoryService
    {
        private readonly IEventCategoryRepository _repo;
        public EventCategoryService(IEventCategoryRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<EventCategoryDto>> GetAllAsync()
        {
            var categories = await _repo.GetAllAsync();

            return categories.Select(c => new EventCategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Slug = c.Slug,
                Description = c.Description,
                ImageUrl = c.ImageUrl
            });
        }

        public async Task<EventCategoryDto?> GetByIdAsync(int id)
        {
            var category = await _repo.GetByIdAsync(id);
            if (category == null) return null;

            return new EventCategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Slug = category.Slug,
                Description = category.Description,
                ImageUrl = category.ImageUrl
            };
        }
        private string GenerateSlug(string name)
        {
            return name
                .Trim()
                .ToLower()
                .Replace(" ", "-")
                .Replace("/", "-")
                .Replace("_", "-");
        }


        public async Task <EventCategoryDto> CreateAsync(EventCategoryDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Category name is required.");

            var category = new EventCategory
            {
                Name = dto.Name.Trim(),
                Slug  = GenerateSlug(dto.Name),
                Description = dto.Description,
                ImageUrl = dto.ImageUrl

            };
            var saved = await _repo.AddAsync(category);

            return new EventCategoryDto
            {
                Id = saved.Id,
                Name = saved.Name,
                Slug = saved.Slug,
                Description = saved.Description,
                ImageUrl = saved.ImageUrl

            };
        }
        public async Task UpdateAsync(int id, EventCategoryDto dto)
        {
            var category = await _repo.GetByIdAsync(id);
            if (category == null) throw new Exception("Category not found");

            category.Name = dto.Name.Trim();
            category.Slug = GenerateSlug(dto.Name);
            category.Description = dto.Description;
            category.ImageUrl = dto.ImageUrl;

            await _repo.UpdateAsync(category);
        }

        public async Task DeleteAsync(int id)
        {
            await _repo.DeleteAsync(id);
        }
    }
}
