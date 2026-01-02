using Applications.Dto;
using Microsoft.AspNetCore.Http;

namespace Applications.Interfaces.IService
{
    public interface IHomeBannerService
    {
        Task<List<HomeBannerDto>> GetAllAsync();
        Task<List<HomeBannerDto>> GetActiveAsync();
        Task<HomeBannerDto?> GetByIdAsync(int id);
        Task AddAsync(HomeBannerDto dto);
        Task UpdateAsync(HomeBannerDto dto);
        Task DeleteAsync(int id);

        Task<string> UploadImageAsync(IFormFile file);
    }
}
