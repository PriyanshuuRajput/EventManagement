using Applications.Dto;
using Applications.Interfaces.IRepository;
using Applications.Interfaces.IService;
using Domains.Entities;

namespace EventBooking_TicketManagement_API.Services
{
    public class VenueService : IVenueService
    {
        private readonly IVenueRepository _venueRepository;
        private readonly ICityRepository _cityRepository;

        public VenueService(IVenueRepository venueRepository, ICityRepository cityRepository)
        {
            _venueRepository = venueRepository;
            _cityRepository = cityRepository;
        }

        // Get all venues
        public async Task<IEnumerable<VenueDto>> GetAllVenuesAsync()
        {
            var venues = await _venueRepository.GetAllAsync();

            return venues.Select(v => new VenueDto
            {
                Id = v.Id,
                VenueName = v.VenueName,
                Address = v.Address,
                Capacity = v.Capacity,
                CityId = v.CityId,
                CityName = v.City?.CityName ?? string.Empty
            });
        }

        // Get single venue
        public async Task<VenueDto?> GetVenueByIdAsync(int id)
        {
            var v = await _venueRepository.GetByIdAsync(id);
            if (v == null) return null;

            return new VenueDto
            {
                Id = v.Id,
                VenueName = v.VenueName,
                Address = v.Address,
                Capacity = v.Capacity,
                CityId = v.CityId,
                CityName = v.City?.CityName ?? string.Empty
            };
        }

        // Add a new venue
        public async Task AddVenueAsync(VenueDto dto)
        {
            var city = await _cityRepository.GetByIdAsync(dto.CityId);
            if (city == null)
                throw new InvalidOperationException($"City with Id {dto.CityId} does not exist.");

            var v = new Venue
            {
                VenueName = dto.VenueName,
                Address = dto.Address,
                Capacity = dto.Capacity,
                CityId = dto.CityId
            };

            await _venueRepository.AddAsync(v);
        }

        // Update existing venue
        public async Task UpdateVenueAsync(int id, VenueDto dto)
        {
            var existing = await _venueRepository.GetByIdAsync(id);
            if (existing == null)
                throw new InvalidOperationException($"Venue with Id {id} does not exist.");

            var city = await _cityRepository.GetByIdAsync(dto.CityId);
            if (city == null)
                throw new InvalidOperationException($"City with Id {dto.CityId} does not exist.");

            existing.VenueName = dto.VenueName;
            existing.Address = dto.Address;
            existing.Capacity = dto.Capacity;
            existing.CityId = dto.CityId;

            await _venueRepository.UpdateAsync(existing);
        }

        // Delete venue
        public async Task DeleteVenueAsync(int id)
        {
            await _venueRepository.DeleteAsync(id);
        }
    }
}
