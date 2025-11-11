using Applications.Dto;

namespace Applications.Interfaces.IService
{
    public interface ICountryService
    {
        Task<List<CountryDto>> GetAllAsync();
        Task<CountryDto?> GetByIdAsync(Guid id);
        Task<CountryDto> CreateAsync(CountryDto dto);
        Task<CountryDto?> UpdateAsync(Guid id, CountryDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
