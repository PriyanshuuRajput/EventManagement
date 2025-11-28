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
                    .ThenInclude(e => e.Seats)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (venue == null)
                throw new InvalidOperationException($"Venue with Id {id} does not exist.");

            int oldCapacity = venue.Capacity;
            int newCapacity = dto.Capacity;

            venue.VenueName = dto.VenueName;
            venue.Address = dto.Address;
            venue.Capacity = dto.Capacity;
            venue.CityId = dto.CityId;

            if (newCapacity > oldCapacity)
            {
                int extraSeats = newCapacity - oldCapacity;

                foreach (var evt in venue.Events)
                {
                    // Generate seat numbers
                    int currentCount = evt.Seats.Count;

                    for (int i = 1; i <= extraSeats; i++)
                    {
                        _context.Seats.Add(new Seat
                        {
                            EventId = evt.Id,
                            Category = "Regular",
                            Price = evt.TicketPrice,
                            IsBooked = false,
                            SeatNumber = $"S{currentCount + i}"
                        });
                    }

                    evt.TotalTickets += extraSeats;
                }
            }


            if (newCapacity < oldCapacity)
            {
                int removeSeats = oldCapacity - newCapacity;

                foreach (var evt in venue.Events)
                {
                    // seats that are free (not booked)
                    var removable = evt.Seats
                        .Where(s => s.IsBooked == false)
                        .OrderByDescending(s => s.Id) // remove last added seats
                        .Take(removeSeats)
                        .ToList();

                    // but what if not enough free seats?
                    if (removable.Count < removeSeats)
                        throw new InvalidOperationException(
                            $"Cannot reduce capacity. Event '{evt.Title}' has too many booked seats.");

                    _context.Seats.RemoveRange(removable);

                    evt.TotalTickets -= removable.Count;
                }
            }

            await _context.SaveChangesAsync();
        }


        // Delete venue
        public async Task DeleteVenueAsync(int id)
        {
            await _venueRepository.DeleteAsync(id);
        }
    }
}
