using Applications.Dto;

namespace Applications.Interfaces.IService
{
    public interface ICityService
    {
        Task<IEnumerable<CityDto>> GetAllCitiesAsync();
        Task<CityDto?> GetCityByIdAsync(int id);
        Task<CityDto?> GetCityByVenueIdAsync(int venueId);

        Task AddCityAsync(CityDto dto);
        Task UpdateCityAsync(int id, CityDto dto);
        Task DeleteCityAsync(int id);
    }
}
