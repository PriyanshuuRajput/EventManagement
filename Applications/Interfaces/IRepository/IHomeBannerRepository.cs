

using Applications.Dto;

namespace Applications.Interfaces.IRepository
{
    public interface IHomeBannerRepository 
    {
        Task<List<HomeBannerDto>> GetAllAsync();

        Task<List<HomeBannerDto>> GetActiveAsync();

        Task<HomeBannerDto?> GetByIdAsync(int id);

        Task AddAsync(HomeBannerDto dto);

        Task UpdateAsync(HomeBannerDto dto);

        Task DeleteAsync(int id);
    }
}
