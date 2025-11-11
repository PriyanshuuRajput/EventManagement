using Applications.Dto;
using Applications.Interfaces.IRepository;
using Applications.Interfaces.IService;
using Domains.Entities;

namespace Applications.Services
{
    public class CountryService : ICountryService
    {
        private readonly ICountryRepository _countryRepo;

        public CountryService(ICountryRepository countryRepo)
        {
            _countryRepo = countryRepo;
        }

        public async Task<List<CountryDto>> GetAllAsync()
        {
            var countries = await _countryRepo.GetAllAsync();
            return countries.Select(c => new CountryDto
            {
                Id = c.Id,
                Name = c.Name,
                IsoCode = c.IsoCode
            }).ToList();
        }

        public async Task<CountryDto?> GetByIdAsync(Guid id)
        {
            var country = await _countryRepo.GetByIdAsync(id);
            if (country == null) return null;

            return new CountryDto
            {
                Id = country.Id,
                Name = country.Name,
                IsoCode = country.IsoCode
            };
        }

        public async Task<CountryDto> CreateAsync(CountryDto dto)
        {
            var country = new Country
            {
                Name = dto.Name,
                IsoCode = dto.IsoCode
            };

            var created = await _countryRepo.CreateAsync(country);

            return new CountryDto
            {
                Id = created.Id,
                Name = created.Name,
                IsoCode = created.IsoCode
            };
        }

        public async Task<CountryDto?> UpdateAsync(Guid id, CountryDto dto)
        {
            var country = new Country
            {
                Id = id,
                Name = dto.Name,
                IsoCode = dto.IsoCode
            };

            var updated = await _countryRepo.UpdateAsync(id, country);
            if (updated == null) return null;

            return new CountryDto
            {
                Id = updated.Id,
                Name = updated.Name,
                IsoCode = updated.IsoCode
            };
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            return await _countryRepo.DeleteAsync(id);
        }
    }
}
