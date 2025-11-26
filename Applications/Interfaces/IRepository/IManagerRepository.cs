using Domains.Entities;

namespace Applications.Interfaces.IRepository
{
    public interface IManagerRepository
    {
        Task<bool> EmailExists(string email);
        Task<AdminUser> CreateAdminUser(AdminUser user);
        Task<Manager> CreateManager(Manager manager);
        Task<Manager?> GetManagerByIdAsync(int id);
        Task<Manager?> GetManagerWithUserAsync(int id);
        Task<Manager?> GetManagerWithUserByUserIdAsync(int userId);
        Task<List<Manager>> GetAllManagersAsync();
        Task<List<Manager>> GetPendingManagersAsync();
        void DeleteManager(Manager manager);
        Task SaveChangesAsync();
    }
}
