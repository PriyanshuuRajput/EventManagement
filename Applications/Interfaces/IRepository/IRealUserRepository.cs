using Domains.Entities;

namespace Applications.Interfaces.IRepository
{
    public interface IRealUserRepository
    {
        Task<RealUser?> GetRealUserAsync(int userId);
        Task CreateAsync(RealUser user);
        Task SaveChangesAsync();
    }

}
