using Applications.Dto;
using Applications.Interfaces.IRepository;
using Applications.Interfaces.IService;

namespace EventBooking_TicketManagement_API.Services
{
    public class HomeBannerService : IHomeBannerService
    {
        public readonly IHomeBannerRepository _homeBannerRepository;
        public HomeBannerService(IHomeBannerRepository homeBannerRepository)
        {
            _homeBannerRepository = homeBannerRepository;

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
    }
}
