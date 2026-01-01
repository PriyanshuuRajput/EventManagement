using Applications.Dto;
using Applications.Interfaces.IRepository;
using Applications.Interfaces.IService;
using Domains.Entities;
using Infrastructures.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace EventBooking_TicketManagement_API.Services
{
    public class VenueService : IVenueService
    {
        private readonly IVenueRepository _venueRepository;
        private readonly ICityRepository _cityRepository;
        private readonly AppDbContext _context;

        public VenueService(IVenueRepository venueRepository, ICityRepository cityRepository, AppDbContext context)
        {
            _venueRepository = venueRepository;
            _cityRepository = cityRepository;
            _context = context;
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
        public async Task<List<VenueDto>> GetVenuesByCityIdAsync(int cityId)
        {
            var venues = await _context.Venues
                .Where(v => v.CityId == cityId)
                .ToListAsync();

            return venues.Select(v => new VenueDto
            {
                Id = v.Id,
                VenueName = v.VenueName,
                Address = v.Address,
                Capacity = v.Capacity,
                CityId = v.CityId,
                CityName = v.City?.CityName
            }).ToList();
        }

        public async Task UpdateVenueAsync(int id, VenueDto dto)
        {
            var venue = await _context.Venues
                .Include(v => v.Events)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (venue == null)
                throw new InvalidOperationException($"Venue with Id {id} does not exist.");

            int newCapacity = dto.Capacity;

            foreach (var evt in venue.Events)
            {
                if (evt.SoldTickets > newCapacity)
                {
                    throw new InvalidOperationException(
                        $"Cannot reduce capacity. Event '{evt.Title}' already has {evt.SoldTickets} sold tickets."
                    );
                }

                evt.TotalTickets = newCapacity;
            }

            venue.VenueName = dto.VenueName;
            venue.Address = dto.Address;
            venue.Capacity = newCapacity;
            venue.CityId = dto.CityId;

            await _context.SaveChangesAsync();
        }



        // Delete venue
        public async Task DeleteVenueAsync(int id)
        {
            await _venueRepository.DeleteAsync(id);
        }
    }
}
