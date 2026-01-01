using Applications.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
