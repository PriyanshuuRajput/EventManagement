using Applications.Dto;

namespace Applications.Interfaces.IService
{
    public interface IVenueService
    {

        Task<IEnumerable<VenueDto>> GetAllVenuesAsync();
        Task<VenueDto?> GetVenueByIdAsync(int id);
        Task<List<VenueDto>> GetVenuesByCityIdAsync(int cityId);

        Task AddVenueAsync(VenueDto venue);
        Task UpdateVenueAsync(int id, VenueDto dto);
        Task DeleteVenueAsync(int id);

    }
}
