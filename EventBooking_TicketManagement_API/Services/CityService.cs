using Applications.Dto;
using Applications.Interfaces.IRepository;
using Applications.Interfaces.IService;
using Domains.Entities;

namespace EventBooking_TicketManagement_API.Services
{
    public class CityService : ICityService
    {
        private readonly ICityRepository _cityRepository;

        public CityService(ICityRepository cityRepository)
        {
            _cityRepository = cityRepository;
        }

        // ✅ Get all cities
        public async Task<IEnumerable<CityDto>> GetAllCitiesAsync()
        {
            var cities = await _cityRepository.GetAllAsync();

            return cities.Select(c => new CityDto
            {
                Id = c.Id,
                CityName = c.CityName,
                StateName = c.State?.Name,
                CountryName = c.State?.Country?.Name
            });
        }

        // ✅ Get city by Id
        public async Task<CityDto?> GetCityByIdAsync(int id)
        {
            var city = await _cityRepository.GetByIdAsync(id);
            if (city == null) return null;

            return new CityDto
            {
                Id = city.Id,
                CityName = city.CityName,
                StateName = city.State?.Name,
                CountryName = city.State?.Country?.Name
            };
        }

        // ✅ Add a city
        public async Task AddCityAsync(CityDto dto)
        {
            var city = new City
            {
                CityName = dto.CityName,
                StateId = dto.StateId
            };

            await _cityRepository.AddAsync(city);
        }

        // ✅ Update a city
        public async Task UpdateCityAsync(int id, CityDto dto)
        {
            var existingCity = await _cityRepository.GetByIdAsync(id);
            if (existingCity == null)
                throw new InvalidOperationException($"City with Id {id} not found");

            existingCity.CityName = dto.CityName;
            existingCity.StateId = dto.StateId;
            await _cityRepository.UpdateAsync(existingCity);
        }

        // ✅ Delete a city
        public async Task DeleteCityAsync(int id)
        {
            await _cityRepository.DeleteAsync(id);
        }
    }
}
