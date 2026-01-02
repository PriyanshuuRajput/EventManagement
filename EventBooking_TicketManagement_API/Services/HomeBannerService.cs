using Applications.Dto;
using Applications.Interfaces.IRepository;
using Applications.Interfaces.IService;

namespace EventBooking_TicketManagement_API.Services
{
    public class HomeBannerService : IHomeBannerService
    {
        public readonly IWebHostEnvironment _env;
        public readonly IHomeBannerRepository _homeBannerRepository;
        public HomeBannerService(IHomeBannerRepository homeBannerRepository , IWebHostEnvironment env)
        {
            _homeBannerRepository = homeBannerRepository;
            _env = env;
        }
        public Task AddAsync(HomeBannerDto dto)
            => _homeBannerRepository.AddAsync(dto);

        public Task DeleteAsync(int id)
            => _homeBannerRepository.DeleteAsync(id);
        

        public Task<List<HomeBannerDto>> GetActiveAsync()
            =>_homeBannerRepository.GetActiveAsync();

        public Task<List<HomeBannerDto>> GetAllAsync()
            =>_homeBannerRepository.GetAllAsync();

        public Task<HomeBannerDto?> GetByIdAsync(int id)
            =>_homeBannerRepository.GetByIdAsync(id);

        public Task UpdateAsync(HomeBannerDto dto)
            =>_homeBannerRepository.UpdateAsync(dto);

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new Exception("File is empty");

            var allowed = new[] { ".jpeg", ".jpg", ".png", ".webp" };
            var ext = Path.GetExtension(file.FileName).ToLower();

            if (!allowed.Contains(ext))
                throw new Exception("Invalid image type");

            var folder = Path.Combine(_env.WebRootPath, "uploads", "carousel");

            Directory.CreateDirectory(folder);

            var fileName= $"{Guid.NewGuid()}{ext}";
            var path = Path.Combine(folder, fileName);

            using var stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/uploads/carousel/{fileName}";

        }
    }
}
